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
            var packet = GetPacket(socket);

            var handler = _requestHandlers[packet.Command];

            var resp = handler.Handle(packet);

            SendPacket(socket, resp);
        }

        public Packet GetPacket(Socket clientSocket)
        {
            var stream = new NetworkStream(clientSocket);

            var buffer = new byte[1024];
            var bytesCount = stream.Read(buffer);

            var packet = Packet.FromBytes(buffer);

            return packet;
        }

        public void SendPacket(Socket clientSocket, Packet packet)
        {
            var stream = new NetworkStream(clientSocket);

            stream.Write(packet.ToBytes());
        }
    }
}
