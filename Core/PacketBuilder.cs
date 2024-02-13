using System.Text;

namespace Core
{
    public class PacketBuilder
    {
        public static IEnumerable<Packet> GetPackets(string content, Status status, Command command, Guid? userId = null)
        {
            var packets = new List<Packet>();

            var packagesCount = (content.Length - 1) / (Packet.MaxDataSize + 1);

            var contentAsArray = content.ToCharArray();

            for (int i = 0; i <= packagesCount; i++)
            {
                var part = new string(contentAsArray.Skip(i * Packet.MaxDataSize).Take(Packet.MaxDataSize).ToArray());

                packets.Add(new Packet
                {
                    UserId = userId.HasValue ? userId.Value : Guid.Empty,
                    Status = status,
                    Command = command,
                    Content = part
                });
            }

            packets.Last().IsLastPacket = true;

            return packets;
        }

        public static IEnumerable<Packet> GetPackets(Stream stream)
        {
            var buffer = new byte[Packet.Size];
            var packets = new List<Packet>();

            while (true)
            {
                stream.Read(buffer);

                var packet = Packet.FromBytes(buffer);

                packets.Add(packet);

                if (packet.IsLastPacket)
                    break;
            }
            
            return packets;
        }

        public static Command GetCommand(IEnumerable<Packet> packets) => packets.First().Command;
        public static Status GetStatus(IEnumerable<Packet> packets) => packets.First().Status;

        public static string GetContent(IEnumerable<Packet> packets)
        {
            var buidler = new StringBuilder();

            foreach(var p in packets)
                buidler.Append(p.Content);

            return buidler.ToString();
        }
    }
}
