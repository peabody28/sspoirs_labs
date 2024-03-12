using Core;

namespace Server.RequestHandlers
{
    public class FileDownloadRequestHandler : IRequestHandler
    {
        public byte[] Handle(byte[] contentBytes, State state, out Status status, out string error)
        {
            var fileName = StringHelper.FromBytes(contentBytes);

            var isSuccess = TryGetFile(fileName, out var respContent, out error);

            status = isSuccess ? Status.FileSended : Status.Error;

            return isSuccess ? respContent : StringHelper.ToBytes(error);
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
                error = "File not found";
                return false;
            }
        }
    }
}
