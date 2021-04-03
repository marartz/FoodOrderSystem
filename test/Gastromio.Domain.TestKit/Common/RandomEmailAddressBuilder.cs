
namespace Gastromio.Domain.TestKit.Common
{
    public static class RandomEmailAddressBuilder
    {
        public static string Build()
        {
            var randomUser = RandomStringBuilder.BuildWithLength(10, "abcdef".ToCharArray());
            var randomHostName = RandomStringBuilder.BuildWithLength(20, "abcdefghijklmnopqrstuvwxyzäüu".ToCharArray());
            return $"{randomUser}@{randomHostName}.de";
        }
    }
}
