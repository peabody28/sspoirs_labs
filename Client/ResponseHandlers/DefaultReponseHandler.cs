using Core;

namespace Client.ResponseHandlers
{
    public class DefaultReponseHandler : IResponseHandler
    {
        public void Handle(Packet packet)
        {
            if (packet.Status.Equals(Status.Ok) || packet.Status.Equals(Status.Error))
            {
                var responseContent = PacketBuilder.GetContentAsString(new[] { packet });
                Console.WriteLine(responseContent);
            }
        }
    }
}
