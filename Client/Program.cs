using Core;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var userId = Guid.NewGuid();

            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var ipAddr = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEndPoint = new(ipAddr, 8080);
            socket.Connect(ipEndPoint);

            while(true)
            {
                Console.Write("Input Command: ");
                var command = Enum.Parse<Command>(Console.ReadLine());

                string content = string.Empty;

                if (command.Equals(Command.Download))
                {
                    Console.Write("Input file path: ");
                    content = Console.ReadLine();
                }
                else if (command.Equals(Command.Upload))
                {
                    Console.Write("Input file for upload: ");
                    var filePath = Console.ReadLine();
                    content = File.ReadAllText(filePath);
                }
                else if (command.Equals(Command.Echo))
                {
                    Console.Write("Input string: ");
                    content = Console.ReadLine();
                }

                var stream = new NetworkStream(socket);

                var requestPackets = PacketBuilder.GetPackets(content, Status.Ok, command, userId);

                foreach(var packet in requestPackets)
                    stream.Write(packet.ToBytes());

                var respPackets = PacketBuilder.GetPackets(stream);
                var status = PacketBuilder.GetStatus(respPackets);
                var responseContent = PacketBuilder.GetContent(respPackets);

                if (status.Equals(Status.FileSended))
                {
                    File.WriteAllText("D:\\text2.txt", responseContent);
                }
                else if(status.Equals(Status.Ok) || status.Equals(Status.Error))
                {
                    Console.WriteLine(responseContent);
                }
            }
        }
    }
}
