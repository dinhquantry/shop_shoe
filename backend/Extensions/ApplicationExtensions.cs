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
            
            return services;
        }
    }
}