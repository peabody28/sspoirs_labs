using Core;

namespace Server.RequestHandlers
{
    public interface IRequestHandler
    {
        string Handle(string content, out Status status, out string error);
    }
}
