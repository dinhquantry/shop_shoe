using backend.Models;
using backend.Security;
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    public static class AppDbSeeder
    {
        public static async Task SeedDefaultDataAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await SeedRolesAsync(context);
        }

        private static async Task SeedRolesAsync(AppDbContext context)
        {
            var roles = await context.PhanQuyens
                .OrderBy(x => x.Id)
                .ToListAsync();

            var adminRole = roles.FirstOrDefault(x => RoleNameHelper.IsAdminRole(x.TenQuyen));
            var customerRole = roles.FirstOrDefault(x => RoleNameHelper.IsCustomerRole(x.TenQuyen));

            if (adminRole is null)
            {
                adminRole = new PhanQuyen
                {
                    TenQuyen = RoleNameHelper.Admin,
                    MoTa = "Tai khoan quan tri he thong"
                };

                context.PhanQuyens.Add(adminRole);
            }
            else
            {
                adminRole.TenQuyen = RoleNameHelper.Admin;
            }

            if (customerRole is null)
            {
                customerRole = new PhanQuyen
                {
                    TenQuyen = RoleNameHelper.Customer,
                    MoTa = "Tai khoan khach hang"
                };

                context.PhanQuyens.Add(customerRole);
            }
            else
            {
                customerRole.TenQuyen = RoleNameHelper.Customer;
            }

            await context.SaveChangesAsync();

            var allRoles = await context.PhanQuyens.ToListAsync();
            var legacyRoles = allRoles
                .Where(x => !RoleNameHelper.IsAdminRole(x.TenQuyen) && !RoleNameHelper.IsCustomerRole(x.TenQuyen))
                .ToList();

            if (legacyRoles.Count == 0)
            {
                return;
            }

            var legacyRoleIds = legacyRoles.Select(x => x.Id).ToList();
            var usersWithLegacyRoles = await context.NguoiDungs
                .Where(x => legacyRoleIds.Contains(x.MaQuyen))
                .ToListAsync();

            foreach (var user in usersWithLegacyRoles)
            {
                user.MaQuyen = customerRole.Id;
            }

            context.PhanQuyens.RemoveRange(legacyRoles);
            await context.SaveChangesAsync();
        }
    }
}
