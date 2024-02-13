using Core;

namespace Server.RequestHandlers
{
    public class EchoRequestHandler : IRequestHandler
    {
        public string Handle(string content, out Status status, out string error)
        {
            error = string.Empty;
            status = Status.Ok;

            return content;
        }
    }
}
