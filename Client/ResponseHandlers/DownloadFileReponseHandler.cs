using Core;

namespace Client.ResponseHandlers
{
    public class DownloadFileReponseHandler : IResponseHandler
    {
        public void Handle(Packet packet)
        {
            if (packet.Status.Equals(Status.FileSended))
            {
                using (var file = new FileStream("C:\\tmp\\downloaded.jpg.tmp", FileMode.Append))
                {
                    file.Write(packet.Content, 0, packet.Length);
                }

                if (packet.IsLastPacket)
                {
                    File.Move("C:\\tmp\\downloaded.jpg.tmp", "C:\\tmp\\downloaded.jpg");
                }
            }
            else if (packet.Status.Equals(Status.Error))
            {
                var responseContent = PacketBuilder.GetContentAsString(new[] { packet });
                Console.WriteLine(responseContent);
            }
        }
    }
}
