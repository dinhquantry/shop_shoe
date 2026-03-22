using System.Globalization;
using System.Text;

namespace backend.Security
{
    public static class RoleNameHelper
    {
        public const string Admin = "Admin";
        public const string Customer = "KhachHang";

        public static bool IsAdminRole(string? value) => NormalizeRole(value) == "admin";

        public static bool IsCustomerRole(string? value)
        {
            var normalized = NormalizeRole(value);
            return normalized is "khachhang" or "customer";
        }

        public static string NormalizeRole(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var normalized = value.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder();

            foreach (var character in normalized)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(character);
                if (unicodeCategory == UnicodeCategory.NonSpacingMark)
                {
                    continue;
                }

                if (char.IsLetterOrDigit(character))
                {
                    builder.Append(char.ToLowerInvariant(character));
                }
            }

            return builder.ToString();
        }
    }
}
