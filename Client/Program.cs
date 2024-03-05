using Core;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var userId = Guid.Parse("d7e62873-0c82-4642-a8cd-e65a0dc9f170");

            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var ipAddr = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEndPoint = new(ipAddr, 8080);
            socket.Connect(ipEndPoint);

            while(true)
            {

                PrintEnum(typeof(Command));
                Console.Write("Input Command: ");

                var command = Enum.Parse<Command>(Console.ReadLine());

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

                var stream = new NetworkStream(socket);

                var requestPackets = PacketBuilder.GetPackets(content, Status.Ok, command, userId);

                foreach(var packet in requestPackets)
                    stream.Write(packet.ToBytes());

                // response
                var buffer = new byte[Packet.Size];
                
                while (true)
                {
                    stream.Read(buffer);

                    var packet = Packet.FromBytes(buffer);

                    if (packet.Status.Equals(Status.FileSended))
                    {
                        using (var file = new FileStream("D:\\downloaded.jpg.tmp", FileMode.Append))
                        {
                            file.Write(packet.Content, 0, packet.Length);
                        }

                        if (packet.IsLastPacket)
                        {
                            File.Move("D:\\downloaded.jpg.tmp", "D:\\downloaded.jpg");
                        }
                    }
                    else if (packet.Status.Equals(Status.Ok) || packet.Status.Equals(Status.Error))
                    {
                        var responseContent = PacketBuilder.GetContentAsString(new[] { packet });
                        Console.WriteLine(responseContent);
                    }

                    if (packet.IsLastPacket)
                        break;
                }
            }
        }
        
        static void PrintEnum(Type enType)
        {
            foreach (var item in Enum.GetNames(enType))
                Console.WriteLine(" - " + item);
        }
    }
}
