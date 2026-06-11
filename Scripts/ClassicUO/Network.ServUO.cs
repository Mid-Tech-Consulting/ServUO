using Server.Network;
using System;
using System.IO;

namespace Server.ClassicUO
{
	public static partial class ClassicUONetwork
	{
		// Field name of the `ToString()` backing field used by the NetState, just makes the logging better
		public const string NSToStringFieldPropertyName = "m_ToString";
		private static void OnReceive(NetState state, PacketReader reader)
		{
			reader.Seek(0, SeekOrigin.Begin);
			var id = reader.ReadByte();
			var result = Handlers[id]?.Invoke(state, reader, reader.Size);
			
			if(result == null || result == false)
			{
				OriginalHandlers[id]?.OnReceive(state, reader);
			}
		}
		
		public static void Log(string str, params object[] args)
		{
#if DEBUG
			Utility.WriteConsoleColor(ConsoleColor.DarkCyan, $"ClassicUONetwork: {str}", args);
#endif
		}
		
		public static void Disconnect(this NetState state, string reason)
		{
			Log($"{reason} - {state}");
			state.Dispose();
		}
		
		public static string ReadUTF8(this PacketReader reader) => reader.ReadUTF8String();
		public static string ReadUTF8Safe(this PacketReader reader, int length) => reader.ReadUTF8StringSafe(length);
		
		public static string GetOrUpdateConfig(string key, string defaultValue)
		{
			if (Config.Find(key) == null)
				Config.Set(key, defaultValue);

			return Config.Get(key, defaultValue);
		}
		
		public static bool GetOrUpdateConfig(string key, bool defaultValue)
		{
			if (Config.Find(key) == null)
				Config.Set(key, defaultValue);

			return Config.Get(key, defaultValue);
		}
	}
}
