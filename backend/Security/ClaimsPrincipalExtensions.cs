using System.Globalization;
using System.Security.Claims;
using System.Text;

namespace backend.Security
{
    public static class ClaimsPrincipalExtensions
    {
        private static readonly HashSet<string> ManagementRoles = new(StringComparer.OrdinalIgnoreCase)
        {
            "admin",
            "administrator",
            "manager",
            "staff",
            "employee",
            "quantri",
            "quanly",
            "nhanvien"
        };

        public static int? GetUserId(this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        public static bool HasManagementAccess(this ClaimsPrincipal user)
        {
            var roles = user.FindAll(ClaimTypes.Role).Select(x => NormalizeRole(x.Value));
            return roles.Any(role => ManagementRoles.Contains(role));
        }

        private static string NormalizeRole(string value)
        {
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
