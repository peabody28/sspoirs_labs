using Core;
using System.Net;
using System.Net.Sockets;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
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

                var reqPacket = new Packet
                {
                    Command = command,
                    Content = content
                };

                socket.Send(reqPacket.ToBytes(), SocketFlags.None);

                var buffer = new byte[1024];
                try
                {
                    socket.Receive(buffer);
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Data recieving error");
                    continue;
                }

                var response = Packet.FromBytes(buffer);

                if(response.Status.Equals(Status.FileSended))
                {
                    File.WriteAllText("D:\\text2.txt", response.Content);
                }
                else if(response.Status.Equals(Status.Ok) || response.Status.Equals(Status.Error))
                {
                    Console.WriteLine(response.Content);
                }
            }
        }
    }
}
