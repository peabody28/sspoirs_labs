using Core;
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

                var respPackets = PacketBuilder.GetPackets(stream);
                var status = PacketBuilder.GetStatus(respPackets);

                if (status.Equals(Status.FileSended))
                {
                    var responseContent = PacketBuilder.GetContent(respPackets);
                    File.WriteAllBytes("D:\\test2.jpg", responseContent);
                }
                else if(status.Equals(Status.Ok) || status.Equals(Status.Error))
                {
                    var responseContent = PacketBuilder.GetContentAsString(respPackets);
                    Console.WriteLine(responseContent);
                }
            }
        }
    }
}
