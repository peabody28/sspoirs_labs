using Core;

namespace Server.RequestHandlers
{
    internal class FileUploadRequestHandler : IRequestHandler
    {
        public string Handle(string content, out Status status, out string error)
        {
            error = string.Empty;
            status = Status.FileRecieved;

            File.WriteAllText("D:\\text.txt", content);

            return null;
        }
    }
}
