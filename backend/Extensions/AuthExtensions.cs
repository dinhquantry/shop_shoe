using System.Text;
using System.Text.Json.Serialization;
using backend.Configuration;
using backend.Security;
using backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace backend.Extensions
{
    public static class AuthExtensions
    {
        public static IServiceCollection AddApiInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddProblemDetails();

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                });

            services.AddScoped<IPasswordHasher<backend.Models.NguoiDung>, PasswordHasher<backend.Models.NguoiDung>>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
            var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>() ?? new JwtSettings();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(AppPolicies.Management, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireAssertion(context => context.User.HasManagementAccess());
                });
            });

            return services;
        }
    }
}
