using Core;

namespace Server.RequestHandlers
{
    internal class FileUploadRequestHandler : IRequestHandler
    {
        public byte[] Handle(byte[] content, State state, out Status status, out string error)
        {
            error = string.Empty;
            status = Status.FileRecieved;

            File.WriteAllBytes("C:\\tmp\\uploadedFile.jpg", content);

            return StringHelper.ToBytes("File uploaded");
        }
    }
}
