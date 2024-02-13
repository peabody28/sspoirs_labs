using Core;
using Server.RequestHandlers;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var tcpServer = new ServerService(IPAddress.Any, 8080, SocketType.Stream, ProtocolType.Tcp);

            tcpServer.Start(HandleRequest);
        }

        private static void HandleRequest(Socket clientSocket)
        {
            while (clientSocket.Connected)
            {
                var requestHandlerResolver = new RequestHandlerResolver();

                requestHandlerResolver.Handle(clientSocket);
            }
        }
    }
}
