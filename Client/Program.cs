using Client.ResponseHandlers;
using Core;
using System.Net;
using System.Net.Sockets;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var responseHandlerResolver = new ResponseHandlerResolver();

            var userId = Guid.Parse("d7e62873-0c82-4642-a8cd-e65a0dc9f170");

            var tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint tcpServerRemotePoint = new(IPAddress.Parse("127.0.0.1"), 8080);

            var udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint udpServerRemotePoint = new(IPAddress.Parse("127.0.0.1"), 5000);

            Task.Run(() =>
            {
                while (true)
                {
                    HealthCheck(udpServerRemotePoint);
                    Thread.Sleep(2000);
                }
            });

            NetworkStream stream = null;
            while (true)
            {
                var state = new State();

                PrintEnum(typeof(Command));
                Console.Write("Input Command: ");

                var command = Enum.Parse<Command>(Console.ReadLine());

                var content = GetContent(command);

                Console.Write("Input Protocol (Tcp/Udp): ");
                var protocol = Enum.Parse<ProtocolType>(Console.ReadLine());

                var requestPackets = PacketBuilder.GetPackets(content, Status.Ok, command, userId);

                // send request
                if (protocol.Equals(ProtocolType.Tcp))
                {
                    if(!tcpSocket.Connected)
                        tcpSocket.Connect(tcpServerRemotePoint);

                    stream = new NetworkStream(tcpSocket);
                    foreach (var packet in requestPackets)
                    {
                        stream.Write(packet.ToBytes());
                        Console.WriteLine($"Package {packet.Id} Sended");
                        Thread.Sleep(50);
                        state.PackagesSended += 1;
                    }
                }
                else if(protocol.Equals(ProtocolType.Udp))
                {
                    foreach (var packet in requestPackets)
                    {
                        udpSocket.SendTo(packet.ToBytes(), udpServerRemotePoint);
                        Console.WriteLine($"Package {packet.Id} Sended");
                        Thread.Sleep(50);
                        state.PackagesSended += 1;
                    }
                }

                // get response
                int packetsCount = 0;
                while (true)
                {
                    var buffer = new byte[Packet.Size];

                    if(protocol.Equals(ProtocolType.Tcp))
                    {
                        stream = new NetworkStream(tcpSocket);

                        stream.Read(buffer);
                    }
                    else if (protocol.Equals(ProtocolType.Udp))
                    {
                        EndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                        udpSocket.ReceiveFrom(buffer, ref sender);
                    }

                    var respPacket = Packet.FromBytes(buffer);
                    if(!state.RecievedPackets.Any(p => p.Id.Equals(respPacket.Id)))
                        state.RecievedPackets.Add(respPacket);

                    if (respPacket.IsLastPacket)
                        packetsCount = respPacket.Id + 1;

                    if (state.RecievedPackets.Count.Equals(packetsCount))
                        break;
                }

                foreach(var respPacket in state.RecievedPackets.OrderBy(p => p.Id))
                    responseHandlerResolver.Handle(respPacket);
            }
        }

        private static bool HealthCheck(IPEndPoint udpServerRemotePoint)
        {
            var udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            var packet = new Packet
            {
                UserId = Guid.Parse("f6b2a2d9-1629-4912-9fe0-78c4e8bc80a5"),
                Command = Command.HealthCheck,
                Content = new byte[0],
                IsLastPacket = true
            };

            udpSocket.SendTo(packet.ToBytes(), udpServerRemotePoint);

            EndPoint sender = new IPEndPoint(IPAddress.Any, 0);

            var buffer = new byte[Packet.Size];

            try
            {
                udpSocket.ReceiveTimeout = 100;
                udpSocket.ReceiveFrom(buffer, ref sender);

                var respPacket = Packet.FromBytes(buffer);

                return respPacket.Status.Equals(Status.Ok);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Server unavailable");
                return false;
            }
        }

        private static byte[] GetContent(Command command)
        {
            byte[] content = null;

            if (command.Equals(Command.Download))
            {
                Console.Write("Input file path: ");
                content = StringHelper.ToBytes(Console.ReadLine());
            }
            else if (command.Equals(Command.Upload))
            {
                Console.Write("Input file for upload: ");
                var filePath = Console.ReadLine();
                content = File.ReadAllBytes(filePath);
            }
            else if (command.Equals(Command.Echo))
            {
                Console.Write("Input string: ");
                content = StringHelper.ToBytes(Console.ReadLine());
            }

            return content;
        }
        
        static void PrintEnum(Type enType)
        {
            foreach (var item in Enum.GetNames(enType))
                Console.WriteLine(" - " + item);
        }
    }
}
