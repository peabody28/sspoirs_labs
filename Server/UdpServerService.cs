using System.Net.Sockets;
using System.Net;

namespace Server
{
    internal class UdpServerService
    {
        private readonly Socket _listenSocket;

        public UdpServerService(IPAddress ipAddress, int port)
        {
            var ipEndPoint = new IPEndPoint(ipAddress, port);

            _listenSocket = new(ipEndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

            _listenSocket.Bind(ipEndPoint);
        }

        public void Start(Action<Socket> action)
        {
            try
            {
                action(_listenSocket);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
