using System.Text;

namespace Core
{
    public class Packet
    {
        public Guid UserId { get; set; }
        public Status Status { get; set; }

        public Command Command { get; set; }

        public int Length { get => Content.Length; set { } }

        public string Content { get; set; }

        public byte[] ToBytes()
        {
            var bytes = new List<byte>();

            bytes.AddRange(UserId.ToByteArray());
            bytes.Add((byte)Status);
            bytes.Add((byte)Command);
            bytes.Add((byte)Length);
            bytes.AddRange(Encoding.UTF8.GetBytes(Content));

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
                Content = Encoding.UTF8.GetString(data.Skip(19).Take(length).ToArray())
            };
        }
    }
}
