using Core;

namespace Server.RequestHandlers
{
    public interface IRequestHandler
    {
        byte[] Handle(byte[] content, out Status status, out string error);
    }
}
