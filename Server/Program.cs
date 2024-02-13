using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{ 
    internal class Program
    {
        private static readonly DateTime _startTime;

        static Program()
        {
            _startTime = DateTime.UtcNow;
        }

        static void Main(string[] args)
        {
            var tcpServer = new ServerService(IPAddress.Any, 8080, SocketType.Stream, ProtocolType.Tcp);

            tcpServer.Start(HandleRequest);
        }

        private static void HandleRequest(Socket clientSocket)
        {
            while (clientSocket.Connected)
            {
                using var stream = new NetworkStream(clientSocket);

                var buffer = new byte[128];
                var bytesCount = stream.Read(buffer);
                var requestString = Encoding.UTF8.GetString(buffer, 0, bytesCount);

                var requestCommandAndArg = requestString.Split(" ");
                var command = requestCommandAndArg[0];

                string response = null;

                if (command.Equals("CLOSE"))
                {
                    clientSocket.Close();
                }
                else if (command.Equals("ECHO"))
                {
                    var argument = requestCommandAndArg[1];
                    response = argument;
                }
                else if (command.Equals("TIME"))
                {
                    var workingTime = DateTime.UtcNow - _startTime;
                    response = workingTime.ToString();
                }
                //else if (command.Equals("DOWNLOAD"))
                //{
                //    var fileName = requestCommandAndArg[1];

                //    if (!TrySendFile(fileName, stream, out var error))
                //    {
                //        stream.WriteByte(1);
                //        response = error;
                //    }
                //}
                //else if (command.Equals("UPLOAD"))
                //{
                //    var workingTime = DateTime.UtcNow - _startTime;
                //    response = workingTime.ToString();
                //}

                if (!string.IsNullOrWhiteSpace(response))
                {
                    var responseBytes = Encoding.UTF8.GetBytes(response + Environment.NewLine);
                    stream.Write(responseBytes);
                }
            }
        }

        private static bool TrySendFile(string fileName, Stream destinationStream, out string error)
        {
            error = string.Empty;
            
            try
            {
                var fileStream = File.OpenRead(fileName);

                if (fileStream != null)
                {
                    destinationStream.WriteByte(0);
                    fileStream.CopyTo(destinationStream);
                    return true;
                }
                else
                {
                    error = "File not exists";
                    return false;
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
    }
}
