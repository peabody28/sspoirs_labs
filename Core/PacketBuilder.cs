namespace Core
{
    public class PacketBuilder
    {
        public static IEnumerable<Packet> GetPackets(byte[] content, Status status, Command command, Guid? userId = null)
        {
            var packets = new List<Packet>();

            var packagesCount = content != null ? (content.Length - 1) / (Packet.MaxDataSize + 1) : 0;

            for (int i = 0; i <= packagesCount; i++)
            {
                var part = content?.Skip(i * Packet.MaxDataSize).Take(Packet.MaxDataSize).ToArray();

                packets.Add(GetPacket(i, part, status, command, userId));
            }

            packets.Last().IsLastPacket = true;

            return packets;
        }

        private static Packet GetPacket(int id, byte[] content, Status status, Command command, Guid? userId = null)
        {
            return new Packet
            {
                UserId = userId.HasValue ? userId.Value : Guid.Empty,
                Id = id,
                Status = status,
                Command = command,
                Content = content ?? new byte[0]
            };
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
        public static Guid GetUserId(IEnumerable<Packet> packets) => packets.First().UserId;

        public static string GetContentAsString(IEnumerable<Packet> packets)
        {
            return StringHelper.FromBytes(GetContent(packets));
        }

        public static byte[] GetContent(IEnumerable<Packet> packets)
        {
            var bytes = new List<byte>();

            foreach (var p in packets.OrderBy(p => p.Id))
                bytes.AddRange(p.Content);

            return bytes.ToArray();
        }
    }
}
