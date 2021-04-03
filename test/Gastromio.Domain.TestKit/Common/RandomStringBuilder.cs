using System.Linq;

namespace Gastromio.Domain.TestKit.Common
{
    public static class RandomStringBuilder
    {
        public static string Build(char[] chars = null)
        {
            return BuildWithRandomLength(50, 50, chars);
        }

        public static string BuildWithLength(int length, char[] chars = null)
        {
            return BuildWithRandomLength(length, length, chars);
        }

        public static string BuildWithRandomLength(int minLength, int maxLength, char[] chars = null)
        {
            var defaultChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
            if (chars == null || chars.Length == 0)
                chars = defaultChars;
            var length = RandomProvider.Random.Next(minLength, maxLength);
            var charArray = Enumerable.Repeat(chars, length)
                .Select(s => s[RandomProvider.Random.Next(s.Length)])
                .ToArray();
            return new string(charArray);
        }
    }
}
