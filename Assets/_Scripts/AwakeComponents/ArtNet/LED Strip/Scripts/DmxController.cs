using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using UnityEngine;

using ArtNet.Sockets;
using ArtNet.Packets;

using AwakeComponents.DebugUI;

namespace AwakeComponents.ArtNet
{
    [ComponentInfo("1.0", "09.10.2024")]
    public class DmxController : MonoBehaviour, IDebuggableComponent
    {
        [Header("ArtNet Configuration")]
        public string remoteIP = "localhost";
        IPEndPoint remote;

        ArtNetSocket artnet;

        [Header("DMX data for debug")]
        [SerializeField] ArtNetDmxPacket dmxToSend;

        [ContextMenu("Send DMX")]
        public void Send()
        {
            artnet.Send(dmxToSend, remote);
        }
        public void Send(short universe, byte[] dmxData)
        {
            dmxToSend.Universe = universe;
            System.Buffer.BlockCopy(dmxData, 0, dmxToSend.DmxData, 0, dmxData.Length);

            artnet.Send(dmxToSend, remote);
        }

        void Start()
        {
            artnet = new ArtNetSocket();
            dmxToSend.DmxData = new byte[512];
            remote = new IPEndPoint(FindFromHostName(remoteIP), ArtNetSocket.Port);
        }

        private void OnDestroy()
        {
            artnet.Close();
        }

        static IPAddress FindFromHostName(string hostname)
        {
            var address = IPAddress.None;
            try
            {
                if (IPAddress.TryParse(hostname, out address))
                    return address;

                var addresses = Dns.GetHostAddresses(hostname);
                for (var i = 0; i < addresses.Length; i++)
                {
                    if (addresses[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        address = addresses[i];
                        break;
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogErrorFormat(
                    "Failed to find IP for :\n host name = {0}\n exception={1}",
                    hostname, e);
            }
            return address;
        }

        public void RenderDebugUI()
        {
            GUILayout.Label("Nothing to show yet");
        }
    }
}
