namespace Gastromio.Domain.TestKit.Common
{
    public static class RandomPhoneNumberBuilder
    {
        public static string Build()
        {
            var randomAreaCode = RandomStringBuilder.BuildWithLength(5, "0123456789".ToCharArray());
            var randomNumber = RandomStringBuilder.BuildWithLength(8, "0123456789".ToCharArray());
            return $"+49 {randomAreaCode} {randomNumber}";
        }
    }
}