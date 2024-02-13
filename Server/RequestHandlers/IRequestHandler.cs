using Core;

namespace Server.RequestHandlers
{
    public interface IRequestHandler
    {
        Packet Handle(Packet packet);
    }
}
