using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using Server;
using Server.Accounting;

namespace Server.Services
{
    public static class DonationProcessor
    {
        private static Timer _pollTimer;
        private static bool _processing;

        // Connection string - loaded from Config/Database.cfg (gitignored)
        private static readonly string ConnectionString =
            Config.Get("Database.ConnectionString", "");

        // How often to poll for pending donations (in seconds)
        private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(30);

        public static void Initialize()
        {
            _pollTimer = Timer.DelayCall(PollInterval, PollInterval, OnTick);
            Console.WriteLine("[DonationProcessor] Started polling for pending donations every {0} seconds.", PollInterval.TotalSeconds);
        }

        private static void OnTick()
        {
            if (_processing)
                return;

            _processing = true;

            Task.Run(async () =>
            {
                var pending = await FetchPendingDonationsAsync();

                // GrantSovereigns touches game state, so it must run on the game thread
                if (pending.Count > 0)
                {
                    Timer.DelayCall(TimeSpan.Zero, () => ProcessOnGameThread(pending));
                }
                else
                {
                    _processing = false;
                }
            });
        }

        private static async Task<List<(long id, string accountName, int sovereigns)>> FetchPendingDonationsAsync()
        {
            var results = new List<(long id, string accountName, int sovereigns)>();

            try
            {
                using (var conn = new NpgsqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();

                    using (var cmd = new NpgsqlCommand(
                        "SELECT id, account_name, sovereigns FROM donations " +
                        "WHERE is_completed = false AND transaction_id IS NOT NULL",
                        conn))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            results.Add((
                                reader.GetInt64(0),
                                reader.GetString(1),
                                reader.GetInt32(2)
                            ));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[DonationProcessor] Error fetching donations: {0}", ex.Message);
            }

            return results;
        }

        private static void ProcessOnGameThread(List<(long id, string accountName, int sovereigns)> pending)
        {
            var completed = new List<long>();

            foreach (var donation in pending)
            {
                if (GrantSovereigns(donation.accountName, donation.sovereigns))
                {
                    completed.Add(donation.id);
                    Console.WriteLine(
                        "[DonationProcessor] Granted {0} sovereigns to account '{1}' (donation #{2})",
                        donation.sovereigns, donation.accountName, donation.id
                    );
                }
                else
                {
                    Console.WriteLine(
                        "[DonationProcessor] Account '{0}' not found, skipping donation #{1}",
                        donation.accountName, donation.id
                    );
                }
            }

            // Mark completed donations on a background thread
            if (completed.Count > 0)
            {
                Task.Run(() => MarkCompletedAsync(completed));
            }
            else
            {
                _processing = false;
            }
        }

        private static async Task MarkCompletedAsync(List<long> ids)
        {
            try
            {
                using (var conn = new NpgsqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();

                    foreach (long id in ids)
                    {
                        using (var cmd = new NpgsqlCommand(
                            "UPDATE donations SET is_completed = true, updated_at = @now WHERE id = @id",
                            conn))
                        {
                            cmd.Parameters.AddWithValue("id", id);
                            cmd.Parameters.AddWithValue("now", DateTime.UtcNow);
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[DonationProcessor] Error marking donations complete: {0}", ex.Message);
            }
            finally
            {
                _processing = false;
            }
        }

        private static bool GrantSovereigns(string accountName, int sovereigns)
        {
            Account acct = Accounts.GetAccount(accountName) as Account;

            return acct != null && acct.DepositSovereigns(sovereigns);
        }
    }
}
