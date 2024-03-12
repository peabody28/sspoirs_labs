using Core;

namespace Server.RequestHandlers
{
    public interface IRequestHandler
    {
        byte[] Handle(byte[] content, State state, out Status status, out string error);
    }
}
