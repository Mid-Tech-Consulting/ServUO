using Server.Network;
using System;
using System.Net;
using System.Reflection;
using static Server.ClassicUO.ClassicUONetwork;

#if !ServUO // MUO
using PacketReader = Server.Network.CircularBufferReader;
#endif

namespace Server.ClassicUO
{
    /// <summary>
    /// Event arguments used when a web identity event occurs.
    /// </summary>
    public class ClassicUOWebIdentityEventArgs
    {
        /// <summary>
        /// Gets the event's timestamp.
        /// </summary>
        public DateTimeOffset Timestamp { get; protected set; }

        /// <summary>
        /// Gets the ClassicUO Web pre-shared secret. This should only be known by your Shard and CUO Web.
        /// </summary>
        public string Secret { get; protected set; }

        /// <summary>
        /// Gets the ClassicUO Web's unique userId.
        /// </summary>
        public string UserId { get; protected set; }

        /// <summary>
        /// Gets the user's IP address as seen by the Game Proxy.
        /// </summary>
        public string ConnectingIp { get; protected set; }

        /// <summary>
        /// Gets the external authentication provider (if any), e.g. Discord.
        /// </summary>
        public string ExternalAuthProvider { get; protected set; }

        /// <summary>
        /// Gets the external authentication username (if any).
        /// </summary>
        public string ExternalAuthUsername { get; protected set; }

        /// <summary>
        /// Gets the external authentication userId (if any).
        /// </summary>
        public string ExternalAuthId { get; protected set; }

        /// <summary>
        /// Gets the ClassicUO Web user role.
        /// </summary>
        public string Role { get; protected set; }

        /// <summary>
        /// Deserializes the web identity event arguments from a packet.
        /// </summary>
        /// <param name="reader">
        /// <para>The reader to use for deserialization.</para>
        /// <para>
        /// <see cref="Server.Network.PacketReader"/> on ServUO <br/>
        /// <see cref="Server.Network.CircularBufferReader"/> on ModernUO
        /// </para>
        /// </param>
        public static ClassicUOWebIdentityEventArgs DeserializeFromPacket(PacketReader reader) =>
            new ClassicUOWebIdentityEventArgs()
            {
                Timestamp = DateTimeOffset.FromUnixTimeSeconds(reader.ReadUInt32()),
                Secret = reader.ReadUTF8(),
                UserId = reader.ReadUTF8(),
                ConnectingIp = reader.ReadUTF8(),
                ExternalAuthProvider = reader.ReadUTF8(),
                ExternalAuthUsername = reader.ReadUTF8(),
                ExternalAuthId = reader.ReadUTF8(),
                Role = reader.ReadUTF8()
            };

        public override string ToString() =>
            string.Format(
                "{{ Timestamp = {0}, Secret = {1}, UserId = {2}, ConnectingIp = {3}, ExternalAuthProvider = {4}, ExternalAuthUsername = {5}, ExternalAuthId = {6}, Role = {7} }}",
                Timestamp,
                Secret,
                UserId,
                ConnectingIp,
                ExternalAuthProvider,
                ExternalAuthUsername,
                ExternalAuthId,
                Role
            );
    }

    /// <summary>
    /// Handles the WebClientIdentity packet that's sent via the 0xA4 SystemInfo packet.
    /// <para>We intercept the 0xA4 SystemInfo packet for the following reasons:</para>
    /// <list type="number">
    /// <item>
    /// <description>
    /// It's an unused packet, only sent by the OSI client, never by CUO itself. The packet handlers for ServUO/RunUO/MUO all read it, but ignore it.
    /// </description>
    /// </item>
    /// <item><description>All emulators support receiving the packet before any Account/GameServer login making it a good candidate to workaround IPLimiter.</description></item>
    /// <item><description>It's large enough for our use, and requires no Core modifications to enable receiving.</description></item>
    /// </list>
    /// </summary>
    /// <remarks>
    /// <para>ServUO: Create a `ClassicUO.cfg` and add the options below.</para>
    /// <para>MUO: Add the options to your `modernuo.json` under `settings`</para>
    /// </remarks>
    public static partial class WebIdentity
    {
        public static readonly string WebIdentitySecret =
            GetOrUpdateConfig("ClassicUO.WebIdentitySecret", "CHANGEME");

        // Kick clients when they fail the secret check, this should be `true` if you have a secret set
        public static readonly bool WebIdentityKickOnBadSecret =
            GetOrUpdateConfig("ClassicUO.WebIdentityKickOnBadSecret", true);

        // This sets the users real IP to the NetState to make sure users IP limits work across both web/desktop
        public static readonly bool WebIdentityIpLimitWorkaround =
            GetOrUpdateConfig("ClassicUO.WebIdentityIpLimitWorkaround", true);


        /// <summary>
        /// Occurs when a ClassicUO Web Identity is received, register an event handler to listen for web identity messages.
        /// </summary>
        /// <remarks>
        /// This event provides a <see cref="ClassicUOWebIdentityEventArgs"/> which contains information about the received identity.
        /// </remarks>
        /// <example>
        /// <code>
        /// WebIdentity.OnWebIdentityReceived += args =>
        /// {
        ///  Console.WriteLine($"Received Identity! {args.UserId}");
        /// };
        /// </code>
        /// </example>
        public static event Action<ClassicUOWebIdentityEventArgs> OnWebIdentityReceived;

        public static void Configure()
        {
            RegisterHandler(0xA4, 149, false, WebIdentityInterceptSystemInfo);
        }

        private static bool WebIdentityInterceptSystemInfo(NetState ns, PacketReader reader, int packetLength)
        {
            var clientType = reader.ReadUTF8Safe(6);
            if (clientType != "CUOWEB")
            {
                Log("Received 0xA4 SystemInfo packet, calling original handler {0}, {1}", ns, clientType);
                return false;
            }

            var version = reader.ReadByte();
            if (version > 1)
            {
                Log("Ident received newer packet version than expected (v{0}) is there an update? {1}", version, ns);
            }

            var args = ClassicUOWebIdentityEventArgs.DeserializeFromPacket(reader);
            var validSecret = WebIdentitySecret == args.Secret;

            if (!validSecret && WebIdentityKickOnBadSecret)
            {
                ns.Disconnect($"Incorrect secret '{args.Secret}' from IP {args.ConnectingIp}, disconnecting");
                return true;
            }

            var identAge = (DateTimeOffset.Now - args.Timestamp).TotalSeconds;
            if (identAge > 30)
            {
                ns.Disconnect($"Timestamp expired, {identAge} seconds old from IP {args.ConnectingIp}, disconnecting");
                return true;
            }

            // Only override the Netstate IP if the secret is valid, otherwise anyone can forge this packet to change IP
            if (validSecret && WebIdentityIpLimitWorkaround)
            {
                if (IPAddress.TryParse(args.ConnectingIp, out var userConnectingIp))
                {
                    ReflectionOverrideNetstateAddress(ns, userConnectingIp);
                }
                else
                {
                    ns.Disconnect($"Ident contained a malformed UserConnectingIp '{args.ConnectingIp}', disconnecting");
                    return true;
                }
            }
            
            OnWebIdentityReceived?.Invoke(args);
            Log("Ident processed {0}", args);

            return true;
        }

        /// <summary>
        /// Overrides the <see cref="NetState.Address"/> property and corrects the <see cref="NetState.ToString"/> output
        /// on <see cref="NetState"/> with the user's IP as seen by CUO Web's Game Proxy.
        /// </summary>
        /// <remarks>
        /// This doesn't affect the underlying Socket, only references to <see cref="NetState.Address"/>.
        /// A much better approach would be to modify <see cref="NetState"/> directly.
        /// Reflection is used so we can just be dropped-in without any emulator core modifications.
        /// </remarks>
        /// <seealso cref="Server.Misc.IPLimiter"/>
        /// <seealso cref="NetState"/>
        private static void ReflectionOverrideNetstateAddress(NetState state, IPAddress addr)
        {
            typeof(NetState).GetField("<Address>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.SetValue(state, addr);

            typeof(NetState).GetField(NSToStringFieldPropertyName, BindingFlags.Instance | BindingFlags.NonPublic)
                ?.SetValue(state, addr.ToString());
        }
    }
}

