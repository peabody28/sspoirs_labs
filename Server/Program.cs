using Server.RequestHandlers;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    internal class Program
    {
        private static RequestHandlerResolver _requestHandlerResolver;
        static void Main(string[] args)
        {
            _requestHandlerResolver = new RequestHandlerResolver();

            var tcpServer = new ServerService(IPAddress.Any, 8080, SocketType.Stream, ProtocolType.Tcp);

            while(true)
                tcpServer.Start(HandleRequest);
        }

        private static void HandleRequest(Socket clientSocket)
        {
            while (clientSocket.Connected)
            {
                _requestHandlerResolver.Handle(clientSocket);
            }
        }
    }
}
