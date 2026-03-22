using System.Security.Claims;

namespace backend.Security
{
    public static class ClaimsPrincipalExtensions
    {
        public static int? GetUserId(this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        public static bool HasManagementAccess(this ClaimsPrincipal user)
        {
            return user.FindAll(ClaimTypes.Role).Any(x => RoleNameHelper.IsAdminRole(x.Value));
        }
    }
}
