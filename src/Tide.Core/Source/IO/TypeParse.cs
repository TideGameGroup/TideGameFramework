using System.Linq;

namespace Tide.Core
{
    public class TypeParse
    {
        public static string Parse<T>()
        {
            string typename = typeof(T).ToString();

            char[] chars = { '.', ',', '\n' };
            string[] strings = typename.Split(chars);

            return strings.Last();
        }
    }
}
