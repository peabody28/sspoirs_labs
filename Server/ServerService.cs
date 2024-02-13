using System.Net;
using System.Net.Sockets;

namespace Server
{
    internal class ServerService
    {
        private readonly Socket _listenSocket;

        public ServerService(IPAddress ipAddress, int port, SocketType socketType, ProtocolType protocolType)
        {
            var ipEndPoint = new IPEndPoint(ipAddress, port);

            _listenSocket = new(ipEndPoint.AddressFamily, socketType, protocolType);

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
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
