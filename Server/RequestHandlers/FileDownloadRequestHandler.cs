using Core;

namespace Server.RequestHandlers
{
    public class FileDownloadRequestHandler : IRequestHandler
    {
        public byte[] Handle(byte[] contentBytes, out Status status, out string error)
        {
            var fileName = StringHelper.FromBytes(contentBytes);

            var isSuccess = TryGetFile(fileName, out var respContent, out error);

            status = isSuccess ? Status.FileSended : Status.Error;

            return respContent;
        }

        private bool TryGetFile(string fileName, out byte[] content, out string error)
        {
            error = string.Empty;
            content = default;

            try
            {
                content = File.ReadAllBytes(fileName);
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
