using System.Security.Cryptography.Xml;

namespace EventHub.AuthService.Models
{
    public class User
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public int RoleId { get; set; }

        public Role? Role { get; set; }

        public bool EmailVerified { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();


    }
}
