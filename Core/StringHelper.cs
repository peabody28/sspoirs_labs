using System.Text;

namespace Core
{
    public class StringHelper
    {
        public static byte[] ToBytes(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        public static string FromBytes(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
