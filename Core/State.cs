namespace Core
{
    public class State
    {
        public Guid UserId { get; set; }

        public Command Command { get; set; }

        public int PackagesSended { get; set; }

        public List<Packet> RecievedPackets { get; set; }

        public State()
        {
            RecievedPackets = new List<Packet>();
        }
    }
}
