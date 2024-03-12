using Core;
using System.Net;
using System.Net.Sockets;

namespace Server.RequestHandlers
{
    public class RequestHandlerResolver 
    {
        private Dictionary<Guid, State> _userState;

        private readonly IDictionary<Command, IRequestHandler> _requestHandlers;

        private object _udpSocketLock = new object();

        public RequestHandlerResolver()
        {
            _requestHandlers = new Dictionary<Command, IRequestHandler>
            {
                { Command.Echo, new EchoRequestHandler() },
                { Command.Time, new TimeRequestHandler() },
                { Command.Upload, new FileUploadRequestHandler() },
                { Command.Download, new FileDownloadRequestHandler() },
                { Command.HealthCheck, new HealthCheckRequestHandler() }
            };

            _userState = new Dictionary<Guid, State>();
        }

        public void Handle(Socket clientSocket)
        {
            while (clientSocket.Connected)
            {
                using var stream = new NetworkStream(clientSocket);
                var reqPackets = PacketBuilder.GetPackets(stream);
                var userId = PacketBuilder.GetUserId(reqPackets);
                var command = PacketBuilder.GetCommand(reqPackets);
                var requestContent = PacketBuilder.GetContent(reqPackets);

                if (!_userState.TryGetValue(userId, out var state))
                {
                    state = new State { UserId = userId, Command = command };
                    _userState.Add(userId, state);
                }

                var handlingResponse = _requestHandlers[command].Handle(requestContent, state, out var status, out var error);

                var responsePackets = PacketBuilder.GetPackets(handlingResponse, status, command, null);

                try
                {
                    foreach (var packet in responsePackets.Skip(state.PackagesSended))
                    {
                        stream.Write(packet.ToBytes());
                        Console.WriteLine($"Package {packet.Id} Sended");
                        Thread.Sleep(50);
                        state.PackagesSended += 1;
                    }

                    _userState.Remove(state.UserId);
                }
                catch
                {
                    Console.WriteLine("Error when sending response");
                }
            }
        }

        public void HandleUdp(Socket socket)
        {
            var packetsCount = 0;
            while(true)
            {
                var buffer = new byte[Packet.Size];
                EndPoint remoteIp = new IPEndPoint(IPAddress.Any, 0);

                lock(_udpSocketLock)
                {
                    try
                    {
                        socket.ReceiveTimeout = 10;
                        socket.ReceiveFrom(buffer, ref remoteIp);
                    }
                    catch
                    {
                        continue;
                    }
                }

                var reqPacket = Packet.FromBytes(buffer);

                if (!_userState.TryGetValue(reqPacket.UserId, out var state))
                {
                    state = new State { UserId = reqPacket.UserId, Command = reqPacket.Command };
                    _userState.Add(reqPacket.UserId, state);
                }

                state.RecievedPackets.Add(reqPacket);

                if (reqPacket.IsLastPacket)
                    packetsCount = reqPacket.Id + 1;

                if (!state.RecievedPackets.Count.Equals(packetsCount))
                    continue;

                var requestContent = PacketBuilder.GetContent(state.RecievedPackets);
                state.RecievedPackets = new List<Packet>();

                var handlingResponse = _requestHandlers[state.Command].Handle(requestContent, state, out var status, out var error);

                var responsePackets = PacketBuilder.GetPackets(handlingResponse, status, state.Command, null);

                Task.Run(() =>
                {
                    try
                    {
                        foreach (var packet in responsePackets.Skip(state.PackagesSended))
                        {
                            lock (_udpSocketLock)
                            {
                                socket.SendTo(packet.ToBytes(), remoteIp);
                            }
                            Console.WriteLine($"Package {state.PackagesSended} Sended");
                            state.PackagesSended += 1;

                            Thread.Sleep(50);

                            if (DateTime.UtcNow - state.LastHandshake > TimeSpan.FromMilliseconds(1000))
                            {
                                state.PackagesSended = state.PackagesSendedWhenHandshake - 1;
                                throw new Exception("User disconnected");
                            }
                        }

                        _userState.Remove(state.UserId);
                    }
                    catch
                    {
                        Console.WriteLine("Error when sending response");
                    }
                });
            }
        }
    }
}
