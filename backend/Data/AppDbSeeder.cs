using backend.Models;
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
            var existingRoleNames = await context.PhanQuyens
                .Select(x => x.TenQuyen.ToLower())
                .ToListAsync();

            var rolesToAdd = new List<PhanQuyen>();

            if (!existingRoleNames.Contains("admin"))
            {
                rolesToAdd.Add(new PhanQuyen
                {
                    TenQuyen = "Admin",
                    MoTa = "Tai khoan quan tri he thong"
                });
            }

            if (!existingRoleNames.Contains("khachhang"))
            {
                rolesToAdd.Add(new PhanQuyen
                {
                    TenQuyen = "KhachHang",
                    MoTa = "Tai khoan khach hang"
                });
            }

            if (!existingRoleNames.Contains("nhanvien"))
            {
                rolesToAdd.Add(new PhanQuyen
                {
                    TenQuyen = "NhanVien",
                    MoTa = "Tai khoan nhan vien"
                });
            }

            if (rolesToAdd.Count == 0)
            {
                return;
            }

            context.PhanQuyens.AddRange(rolesToAdd);
            await context.SaveChangesAsync();
        }
    }
}
