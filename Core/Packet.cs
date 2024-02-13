using Core;
using System.Text;

namespace Server
{
    public class Packet
    {
        public Status Status { get; set; }

        public Command Command { get; set; }

        public int Length { get => Content.Length; set => Length = value; }

        public string Content { get; set; }

        public byte[] ToBytes()
        {
            var bytes = new List<byte>();

            bytes.Add((byte)Status);
            bytes.Add((byte)Command);
            bytes.Add((byte)Length);
            bytes.AddRange(Encoding.UTF8.GetBytes(Content));

            return bytes.ToArray();
        }

        public static Packet FromBytes(byte[] data)
        {
            return new Packet
            {
                Status = (Status)data[0],
                Command = (Command)data[1],
                Length = (int)data[2],
                Content = Encoding.UTF8.GetString(data.Skip(3).Take((int)data[2]).ToArray())
            };
        }
    }
}
