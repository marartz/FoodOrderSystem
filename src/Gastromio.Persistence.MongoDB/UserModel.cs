using System;

namespace Gastromio.Persistence.MongoDB
{
    public class UserModel
    {
        public Guid Id { get; set; }

        public string Role { get; set; }

        public string Email { get; set; }

        public byte[] PasswordSalt { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordResetCode { get; set; }

        public DateTime? PasswordResetExpiration { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }

        public Guid UpdatedBy { get; set; }
    }
}
