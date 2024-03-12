using Core;

namespace Client.ResponseHandlers
{
    public interface IResponseHandler
    {
        void Handle(Packet packet);
    }
}
