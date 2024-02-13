using System.Net;
using System.Net.Sockets;
using System.Text;

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

                // //"DOWNLOAD D:\\text.txt";

                var message = Console.ReadLine();
                var messageBytes = Encoding.UTF8.GetBytes(message);

                socket.Send(messageBytes, SocketFlags.None);

                var buffer = new byte[1024];

                var recievedBytes = socket.Receive(buffer);

                File.WriteAllBytes("D:\\text2.txt", buffer.Take(recievedBytes).ToArray());
            }
            
        }
    }
}
