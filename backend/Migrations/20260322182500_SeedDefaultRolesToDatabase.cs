using backend.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260322182500_SeedDefaultRolesToDatabase")]
    public partial class SeedDefaultRolesToDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                SET NOCOUNT ON;

                DECLARE @AdminId INT;
                DECLARE @CustomerId INT;

                SELECT TOP (1) @AdminId = Id
                FROM PhanQuyens
                WHERE TenQuyen = N'Admin';

                IF @AdminId IS NULL
                BEGIN
                    INSERT INTO PhanQuyens (TenQuyen, MoTa)
                    VALUES (N'Admin', N'Tai khoan quan tri he thong');

                    SET @AdminId = CAST(SCOPE_IDENTITY() AS INT);
                END
                ELSE
                BEGIN
                    UPDATE PhanQuyens
                    SET MoTa = N'Tai khoan quan tri he thong'
                    WHERE Id = @AdminId;
                END;

                SELECT TOP (1) @CustomerId = Id
                FROM PhanQuyens
                WHERE TenQuyen = N'KhachHang';

                IF @CustomerId IS NULL
                BEGIN
                    INSERT INTO PhanQuyens (TenQuyen, MoTa)
                    VALUES (N'KhachHang', N'Tai khoan khach hang');

                    SET @CustomerId = CAST(SCOPE_IDENTITY() AS INT);
                END
                ELSE
                BEGIN
                    UPDATE PhanQuyens
                    SET MoTa = N'Tai khoan khach hang'
                    WHERE Id = @CustomerId;
                END;

                UPDATE NguoiDungs
                SET MaQuyen = @CustomerId
                WHERE MaQuyen IN
                (
                    SELECT Id
                    FROM PhanQuyens
                    WHERE TenQuyen NOT IN (N'Admin', N'KhachHang')
                );

                DELETE FROM PhanQuyens
                WHERE TenQuyen NOT IN (N'Admin', N'KhachHang');
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DELETE FROM PhanQuyens
                WHERE TenQuyen IN (N'Admin', N'KhachHang')
                  AND Id NOT IN
                  (
                      SELECT DISTINCT MaQuyen
                      FROM NguoiDungs
                  );
                """);
        }
    }
}
