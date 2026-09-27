
namespace EventHub.AuthService.Validators
{
    public class PasswordValidator
    {
        public const int MinimumLength = 8;

        public static bool IsValid(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            if (password.Length < MinimumLength) return false;

            if (password.Any(char.IsWhiteSpace)) return false;

            if (!password.Any(char.IsUpper)) return false;

            if (!password.Any(char.IsLower)) return false;

            if (!password.Any(char.IsDigit)) return false;

            if (!password.Any(IsSpecialCharacter)) return false;

            return true;
        }

        public static string GetValidationMessage(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return "Mật khẩu không được để trống.";

            if (password.Length < MinimumLength) return $"Mật khẩu phải có ít nhất {MinimumLength} ký tự.";

            if (password.Any(char.IsWhiteSpace)) return "Mật khẩu không được chứa khoảng trắng.";

            if (!password.Any(char.IsUpper)) return "Mật khẩu ít nhất một chữ cái viết hoa.";

            if (!password.Any(char.IsLower)) return "Mật khẩu ít nhất 1 chữ cái viết thường.";

            if (!password.Any(char.IsDigit)) return "Mật khẩu ít nhất cho 1 chữ số";

            if (!password.Any(IsSpecialCharacter)) return "Mật khẩu ít nhất có 1 ký tự đặc biệt";

            return string.Empty;
        }

        private static bool IsSpecialCharacter(char character)
        {
            return !char.IsLetterOrDigit(character) && !char.IsWhiteSpace(character);
        }
    }
}
