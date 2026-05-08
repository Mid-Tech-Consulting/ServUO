using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Server;
using Server.Commands;

namespace Server.Engines.Gathering
{
    /// <summary>
    /// Admin helper: [clilocsearch <text> -- prints up to 30 cliloc entries
    /// whose text contains the given keyword (case-insensitive). Useful for
    /// picking a context-menu label cliloc number.
    ///
    /// Reads cliloc.enu directly because the bundled Ultima.StringList loader
    /// reads the per-entry length as signed Int16 and chokes on any entry
    /// longer than 32767 bytes ("Non-negative number required" error).
    /// </summary>
    public static class ClilocSearchCommand
    {
        private static Dictionary<int, string> _Cliloc;

        public static void Initialize()
        {
            CommandSystem.Register("clilocsearch", AccessLevel.Administrator, OnCommand);
            CommandSystem.Register("clilocinfo", AccessLevel.Administrator, OnInfoCommand);
        }

        // Diagnostics: shows where the loaded data is coming from and what
        // the parser actually produced. Run this if [clilocsearch returns
        // suspiciously few results.
        [Usage("clilocinfo")]
        [Description("Shows cliloc loader diagnostics.")]
        private static void OnInfoCommand(CommandEventArgs e)
        {
            if (e.Mobile == null)
                return;

            string path = Ultima.Files.GetFilePath("cliloc.enu");
            if (path == null)
            {
                e.Mobile.SendMessage(0x22, "cliloc.enu not found via Ultima.Files.GetFilePath.");
                return;
            }

            long fileSize = new FileInfo(path).Length;
            e.Mobile.SendMessage(0x40, "cliloc.enu path: {0}", path);
            e.Mobile.SendMessage(0x40, "file size: {0:N0} bytes", fileSize);

            // Dump first 32 bytes in hex so we can identify the file format
            // (old plain vs. modern Huffman-compressed).
            try
            {
                byte[] head = new byte[32];
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    int read = fs.Read(head, 0, head.Length);
                    var sb = new StringBuilder("first bytes: ");
                    for (int i = 0; i < read; i++)
                        sb.AppendFormat("{0:X2} ", head[i]);
                    e.Mobile.SendMessage(0x35, sb.ToString());
                }
            }
            catch (Exception ex)
            {
                e.Mobile.SendMessage(0x22, "Hex dump failed: {0}", ex.Message);
            }

            if (_Cliloc == null)
            {
                try { _Cliloc = LoadClilocs(); }
                catch (Exception ex)
                {
                    e.Mobile.SendMessage(0x22, "Load error: {0}", ex.Message);
                    return;
                }
            }

            e.Mobile.SendMessage(0x40, "loaded entries: {0:N0}", _Cliloc.Count);

            int min = int.MaxValue, max = int.MinValue;
            foreach (int k in _Cliloc.Keys)
            {
                if (k < min) min = k;
                if (k > max) max = k;
            }

            if (_Cliloc.Count > 0)
                e.Mobile.SendMessage(0x40, "cliloc number range: {0} - {1}", min, max);

            // Probe a few well-known cliloc IDs to verify lookup
            int[] probe = new[] { 1115022, 1154679, 1044104, 1044105 };
            string[] probeNames = new[] { "Open Titles Menu", "Vendor Search", "Lumberjacking", "Mining" };
            for (int i = 0; i < probe.Length; i++)
            {
                if (_Cliloc.TryGetValue(probe[i], out string text))
                    e.Mobile.SendMessage(0x35, "probe {0} ({1}): \"{2}\"", probe[i], probeNames[i], text);
                else
                    e.Mobile.SendMessage(0x22, "probe {0} ({1}): NOT FOUND", probe[i], probeNames[i]);
            }
        }

        [Usage("clilocsearch <text>")]
        [Description("Lists cliloc numbers whose text contains the keyword.")]
        private static void OnCommand(CommandEventArgs e)
        {
            if (e.Mobile == null)
                return;

            if (string.IsNullOrWhiteSpace(e.ArgString))
            {
                e.Mobile.SendMessage(0x22, "Usage: [clilocsearch <text>");
                return;
            }

            string keyword = e.ArgString.Trim();

            if (_Cliloc == null)
            {
                try
                {
                    _Cliloc = LoadClilocs();
                }
                catch (Exception ex)
                {
                    e.Mobile.SendMessage(0x22, "Failed to load cliloc data: {0}", ex.Message);
                    return;
                }
            }

            if (_Cliloc.Count == 0)
            {
                e.Mobile.SendMessage(0x22, "Cliloc data is empty -- check that cliloc.enu exists in your client data path.");
                return;
            }

            int matches = 0;
            const int MaxResults = 30;

            e.Mobile.SendMessage(0x40, "Searching {0} clilocs for '{1}'...", _Cliloc.Count, keyword);

            foreach (var kv in _Cliloc)
            {
                if (string.IsNullOrEmpty(kv.Value))
                    continue;

                if (kv.Value.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) < 0)
                    continue;

                e.Mobile.SendMessage(0x35, "{0}: {1}", kv.Key, kv.Value);
                matches++;

                if (matches >= MaxResults)
                {
                    e.Mobile.SendMessage(0x22, "(stopped at {0} results -- refine keyword for more)", MaxResults);
                    break;
                }
            }

            if (matches == 0)
                e.Mobile.SendMessage(0x22, "No cliloc entries matched '{0}'.", keyword);
            else
                e.Mobile.SendMessage(0x40, "Found {0} matching cliloc entries.", matches);
        }

        private static Dictionary<int, string> LoadClilocs()
        {
            string path = Ultima.Files.GetFilePath("cliloc.enu");
            if (path == null || !File.Exists(path))
                throw new FileNotFoundException("cliloc.enu not found in client data path");

            var dict = new Dictionary<int, string>();

            using (var bin = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                bin.ReadInt32();  // header1
                bin.ReadInt16();  // header2

                while (bin.BaseStream.Position < bin.BaseStream.Length)
                {
                    int number = bin.ReadInt32();
                    bin.ReadByte();  // flag
                    int length = bin.ReadUInt16();  // unsigned -- the bug in Ultima.StringList

                    if (length <= 0)
                        continue;

                    byte[] buf = bin.ReadBytes(length);
                    string text = Encoding.UTF8.GetString(buf);

                    dict[number] = text;
                }
            }

            return dict;
        }
    }
}
