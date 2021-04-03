namespace Gastromio.Domain.TestKit.Common
{
    public static class RandomWebSiteBuilder
    {
        public static string Build()
        {
            var randomHostName = RandomStringBuilder.BuildWithLength(20, "abcdefghijklmnopqrstuvwxyzäüu".ToCharArray());
            var randomSuffix = RandomStringBuilder.BuildWithLength(8, "abcdef".ToCharArray());
            return $"https://www.{randomHostName}.de/{randomSuffix}";
        }
    }
}
