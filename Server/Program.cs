using Server.RequestHandlers;
using System.Net;

namespace Server
{
    internal class Program
    {
        private static RequestHandlerResolver _requestHandlerResolver;

        static void Main(string[] args)
        {
            _requestHandlerResolver = new RequestHandlerResolver();

            Task.Run(() =>
            {
                var tcpServer = new TcpServerService(IPAddress.Any, 8080);
                while (true)
                    tcpServer.Start(_requestHandlerResolver.Handle);
            });


            Task.Run(() =>
            {
                var udpServer = new UdpServerService(IPAddress.Any, 5000);
                while (true)
                    udpServer.Start(_requestHandlerResolver.HandleUdp);
            });

            Console.ReadKey();
        }
    }
}
