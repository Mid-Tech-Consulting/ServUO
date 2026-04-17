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

        // Per-command DB timeout. Anything that takes longer is treated as failure
        // so a hung Postgres can never wedge the polling loop forever.
        private const int CommandTimeoutSeconds = 5;

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
                try
                {
                    var pending = await FetchPendingDonationsAsync().ConfigureAwait(false);

                    if (pending.Count > 0)
                    {
                        // GrantSovereigns touches game state, so it must run on the game thread.
                        // _processing is reset at the end of the game-thread work or after the
                        // background mark-complete task finishes.
                        Timer.DelayCall(TimeSpan.Zero, () => ProcessOnGameThread(pending));
                    }
                    else
                    {
                        _processing = false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[DonationProcessor] Poll task failed: {0}", ex);
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
                    await conn.OpenAsync().ConfigureAwait(false);

                    using (var cmd = new NpgsqlCommand(
                        "SELECT id, account_name, sovereigns FROM donations " +
                        "WHERE is_completed = false AND transaction_id IS NOT NULL",
                        conn))
                    {
                        cmd.CommandTimeout = CommandTimeoutSeconds;

                        using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
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

            try
            {
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
            }
            catch (Exception ex)
            {
                Console.WriteLine("[DonationProcessor] Error granting sovereigns: {0}", ex);
            }

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
                    await conn.OpenAsync().ConfigureAwait(false);

                    using (var cmd = new NpgsqlCommand(
                        "UPDATE donations SET is_completed = true, updated_at = @now WHERE id = ANY(@ids)",
                        conn))
                    {
                        cmd.CommandTimeout = CommandTimeoutSeconds;
                        cmd.Parameters.AddWithValue("now", DateTime.UtcNow);
                        cmd.Parameters.AddWithValue("ids", ids.ToArray());

                        await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
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
