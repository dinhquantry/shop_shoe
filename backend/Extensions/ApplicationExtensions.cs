using backend.Profiles;
namespace backend.Extensions
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddCoreApplicationServices(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddAutoMapper(cfg => { }, typeof(MappingProfile));
            // Sau này thêm cấu hình CORS, Dependency Injection ở đây
            services.AddCors(options =>
            {
                options.AddPolicy("AllowNextJsApp", policy =>
                {
                    policy.WithOrigins("http://localhost:3000") // Cho phép Next.js gọi vào
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });

            return services;
        }
    }
}