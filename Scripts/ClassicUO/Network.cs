using Server.Network;

#if !ServUO // MUO
using PacketReader = Server.Network.CircularBufferReader;
using PacketHandlers = Server.Network.IncomingPackets;
#endif

namespace Server.ClassicUO
{
    public static unsafe partial class ClassicUONetwork
    {
        private static PacketHandler[] OriginalHandlers { get; } = new PacketHandler[0xFF];
        private static HandlerFn[] Handlers { get; } = new HandlerFn[0xFF];

        public delegate bool HandlerFn(NetState ns, PacketReader reader, int packetLength);

        public static void RegisterHandler(int id, int len, bool inGame, HandlerFn handler)
        {
            OriginalHandlers[id] = PacketHandlers.GetHandler(id);
            Handlers[id] = handler;
            PacketHandlers.Register(id, len, inGame, OnReceive);
        }
    }
}
