using System.Net;
using System.Net.Sockets;

namespace Server
{
    internal class TcpServerService
    {
        private readonly Socket _listenSocket;

        public TcpServerService(IPAddress ipAddress, int port)
        {
            var ipEndPoint = new IPEndPoint(ipAddress, port);

            _listenSocket = new(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            _listenSocket.Bind(ipEndPoint);
            _listenSocket.Listen(100);
        }

        public void Start(Action<Socket> action)
        {
            var childSocket = _listenSocket.Accept();

            try
            {
                action(childSocket);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
