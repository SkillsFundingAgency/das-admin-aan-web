using SFA.DAS.Admin.Aan.Web.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Admin.Aan.Web.AppStart;

[ExcludeFromCodeCoverage]
public static class AddSessionExtension
{
    public static IServiceCollection AddSession(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(10);
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.IsEssential = true;
        });

        if (configuration["EnvironmentName"].Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase))
        {
            services.AddDistributedMemoryCache();
        }
        else
        {
            services.AddStackExchangeRedisCache(options =>
            {
                var applicationCopnfiguration = configuration.GetSection(nameof(ApplicationConfiguration)).Get<ApplicationConfiguration>();
                options.Configuration = applicationCopnfiguration.RedisConnectionString;
            });
        }

        return services;
    }
}
