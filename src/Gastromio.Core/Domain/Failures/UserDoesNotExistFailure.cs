using Gastromio.Core.Common;

namespace Gastromio.Core.Domain.Failures
{
    public class UserDoesNotExistFailure : Failure
    {
        public override string ToString()
        {
            return "Benutzer existiert nicht";
        }
    }
}
