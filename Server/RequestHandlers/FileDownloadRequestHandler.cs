using Core;

namespace Server.RequestHandlers
{
    public class FileDownloadRequestHandler : IRequestHandler
    {
        public Packet Handle(Packet packet)
        {
            var fileName = packet.Content;

            var isSuccess = TryGetFile(fileName, out var content, out var error);

            var respPacket = new Packet
            {
                Status = isSuccess ? Status.FileSended : Status.Error,
                Content = isSuccess ? content : error
            };

            return respPacket;
        }

        private bool TryGetFile(string fileName, out string content, out string error)
        {
            error = string.Empty;
            content = string.Empty;

            try
            {
                content = File.ReadAllText(fileName);
                return true;
            }
            catch (Exception ex)
            {
                error = "An error occured";
                return false;
            }
        }
    }
}
