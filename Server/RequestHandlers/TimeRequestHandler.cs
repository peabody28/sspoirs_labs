using Core;

namespace Server.RequestHandlers
{
    public class TimeRequestHandler : IRequestHandler
    {
        private readonly DateTime _startTime;

        public TimeRequestHandler()
        {
            _startTime = DateTime.Now;
        }

        public Packet Handle(Packet packet)
        {
            var workingTime = DateTime.UtcNow - _startTime;

            var respPacket = new Packet
            {
                Status = Status.Ok,
                Content = workingTime.ToString()
            };

            return respPacket;
        }
    }
}
