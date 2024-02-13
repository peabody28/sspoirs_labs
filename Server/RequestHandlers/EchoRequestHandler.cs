using Core;

namespace Server.RequestHandlers
{
    public class EchoRequestHandler : IRequestHandler
    {
        public Packet Handle(Packet packet)
        {
            var respPacket = new Packet
            {
                Status = Status.Ok,
                Content = packet.Content,
            };

            return respPacket;
        }
    }
}
