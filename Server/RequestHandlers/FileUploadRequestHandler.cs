using Core;

namespace Server.RequestHandlers
{
    internal class FileUploadRequestHandler : IRequestHandler
    {
        public Packet Handle(Packet packet)
        {
            File.WriteAllText("D:\\text.txt", packet.Content);

            return new Packet
            {
                Status = Status.FileRecieved
            };
        }
    }
}
