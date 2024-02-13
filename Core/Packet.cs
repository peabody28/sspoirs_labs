using System.Text;

namespace Core
{
    public class Packet
    {
        public static readonly int Size = 210 + 16 + 3 + 1;
        public static readonly int MaxDataSize = 210;
        public Guid UserId { get; set; }
        public Status Status { get; set; }

        public Command Command { get; set; }

        public int Length { get => Content.Length; set { } }

        public byte[] Content { get; set; }

        public bool IsLastPacket { get; set; }

        public byte[] ToBytes()
        {
            var bytes = new List<byte>();

            bytes.AddRange(UserId.ToByteArray());
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
            var length = data[18];

            return new Packet
            {
                UserId = new Guid(data.Take(16).ToArray()),
                Status = (Status)data[16],
                Command = (Command)data[17],
                Length = length,
                IsLastPacket = data[19] == 1,
                Content = data.Skip(20).Take(length).ToArray(),
            };
        }
    }
}
