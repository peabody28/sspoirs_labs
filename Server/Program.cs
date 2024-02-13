using Core;
using Server.RequestHandlers;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    internal class Program
    {
        private static readonly DateTime _startTime;

        private static Dictionary<Guid, State> _userState;

        static Program()
        {
            _userState = new();
            _startTime = DateTime.UtcNow;
        }

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
