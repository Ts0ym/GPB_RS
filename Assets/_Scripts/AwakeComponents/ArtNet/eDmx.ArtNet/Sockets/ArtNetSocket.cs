using ArtNet.IO;
using ArtNet.Packets;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace ArtNet.Sockets
{
    public class ArtNetSocket : Socket
    {
        public const int Port = 6454;

        public event UnhandledExceptionEventHandler UnhandledException;

        public ArtNetSocket()
            : base(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, System.Net.Sockets.ProtocolType.Udp)
        {        }

        #region Information

        private bool portOpen = false;

        public bool PortOpen
        {
            get { return portOpen; }
            set { portOpen = value; }
        }

        private DateTime? lastPacket = null;

        public DateTime? LastPacket
        {
            get { return lastPacket; }
            protected set { lastPacket = value; }
        }

        #endregion
   
        protected void OnUnhandledException(Exception ex)
        {
            if (UnhandledException != null) UnhandledException(this, new UnhandledExceptionEventArgs((object)ex, false));
        }

        #region Sending

        public void Send(ArtNetPacket packet, IPEndPoint remote)
        {
            SendTo(packet.ToArray(), remote);
        }
      
        #endregion

        protected override void Dispose(bool disposing)
        {
            PortOpen = false;

            base.Dispose(disposing);
        }
    }
}
