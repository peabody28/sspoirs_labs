using Core;
using System.Net.Sockets;

namespace Server.RequestHandlers
{
    public class RequestHandlerResolver 
    {
        private Dictionary<Guid, State> _userState;

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

            _userState = new Dictionary<Guid, State>();
        }

        public void Handle(Socket socket)
        {
            var stream = new NetworkStream(socket);

            var reqPackets = PacketBuilder.GetPackets(stream);
            var userId = PacketBuilder.GetUserId(reqPackets);
            var command = PacketBuilder.GetCommand(reqPackets);
            var requestContent = PacketBuilder.GetContent(reqPackets);

            if (!_userState.TryGetValue(userId, out var state))
            {
                state = new State();
                _userState.Add(userId, state);
            }

            var handlingResponse = _requestHandlers[command].Handle(requestContent, out var status, out var error);

            var responsePackets = PacketBuilder.GetPackets(handlingResponse, status, Command.Answer, null);

            try
            {
                foreach (var packet in responsePackets.Skip(state.PackagesSended))
                {
                    stream.Write(packet.ToBytes());
                    Console.WriteLine($"Package {state.PackagesSended} Sended");
                    state.PackagesSended += 1;
                    Thread.Sleep(100);
                }

                state.PackagesSended = 0;
            }
            catch
            {
                Console.WriteLine("Error when sending response");
            }
        }
    }
}
