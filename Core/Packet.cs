namespace Core
{
    public class Packet
    {
        public static readonly int Size = 16 + 4 + 1 + 1 + 1 + 1 + 210;
        public static readonly int MaxDataSize = 210;

        public Guid UserId { get; set; }
        public int Id { get; set; }
        public Status Status { get; set; }
        public Command Command { get; set; }
        public int Length { get => Content.Length; set { } }
        public bool IsLastPacket { get; set; }
        public byte[] Content { get; set; }

        public byte[] ToBytes()
        {
            var bytes = new List<byte>();

            bytes.AddRange(UserId.ToByteArray());
            bytes.AddRange(BitConverter.GetBytes(Id));
            bytes.Add((byte)Status);
            bytes.Add((byte)Command);
            bytes.Add((byte)Length);
            bytes.Add((byte)(IsLastPacket ? 1 : 0));

            var content = new byte[MaxDataSize];
            Content.CopyTo(content, 0);

            bytes.AddRange(content);

            return bytes.ToArray();
        }

        public static Packet FromBytes(byte[] data)
        {
            var length = data[22];

            return new Packet
            {
                UserId = new Guid(data.Take(16).ToArray()),
                Id = BitConverter.ToInt32(data.Skip(16).Take(4).ToArray(), 0),
                Status = (Status)data[20],
                Command = (Command)data[21],
                Length = length,
                IsLastPacket = data[23] == 1,
                Content = data.Skip(24).Take(length).ToArray(),
            };
        }
    }
}
