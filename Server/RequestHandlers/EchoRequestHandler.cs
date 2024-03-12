using Core;

namespace Server.RequestHandlers
{
    public class EchoRequestHandler : IRequestHandler
    {
        public byte[] Handle(byte[] content, State state, out Status status, out string error)
        {
            error = string.Empty;
            status = Status.Ok;

            return content;
        }
    }
}
