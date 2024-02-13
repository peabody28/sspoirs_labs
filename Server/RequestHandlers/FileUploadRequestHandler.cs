using Core;

namespace Server.RequestHandlers
{
    internal class FileUploadRequestHandler : IRequestHandler
    {
        public byte[] Handle(byte[] content, out Status status, out string error)
        {
            error = string.Empty;
            status = Status.FileRecieved;

            File.WriteAllBytes("D:\\uploadedFile.jpg", content);

            return null;
        }
    }
}
