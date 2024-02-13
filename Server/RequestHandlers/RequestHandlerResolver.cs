using Core;
using System.Net.Sockets;

namespace Server.RequestHandlers
{
    public class RequestHandlerResolver 
    {
        private readonly IDictionary<Command, IRequestHandler> _requestHandlers;

        public RequestHandlerResolver()
        {
            _requestHandlers = new Dictionary<Command, IRequestHandler>
            {
                { Command.Echo, new EchoRequestHandler() },
                { Command.Time, new TimeRequestHandler() },
                { Command.Upload, new FileUploadRequestHandler() },
                { Command.Download, new FileDownloadRequestHandler() },
            };
        }

        public void Handle(Socket socket)
        {
            var stream = new NetworkStream(socket);

            var reqPackets = PacketBuilder.GetPackets(stream);
            var command = PacketBuilder.GetCommand(reqPackets);
            var requestContent = PacketBuilder.GetContent(reqPackets);

            var handlingResponse = _requestHandlers[command].Handle(requestContent, out var status, out var error);

            var responsePackets = PacketBuilder.GetPackets(handlingResponse, status, Command.Answer, null);

            foreach(var packet in responsePackets)
                stream.Write(packet.ToBytes());
        }
    }
}
