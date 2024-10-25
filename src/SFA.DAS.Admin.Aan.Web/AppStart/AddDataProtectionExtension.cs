using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.DataProtection;
using SFA.DAS.Admin.Aan.Web.Configuration;
using StackExchange.Redis;

namespace SFA.DAS.Admin.Aan.Web.AppStart
{
    [ExcludeFromCodeCoverage]
    public static class AddDataProtectionExtension
    {
        public static void AddDataProtection(this IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.GetSection(nameof(ApplicationConfiguration))
                .Get<ApplicationConfiguration>();

            if (config != null
                && !string.IsNullOrEmpty(config.DataProtectionKeysDatabase)
                && !string.IsNullOrEmpty(config.RedisConnectionString))
            {
                var redisConnectionString = config.RedisConnectionString;
                var dataProtectionKeysDatabase = config.DataProtectionKeysDatabase;

                var configurationOptions = ConfigurationOptions.Parse($"{redisConnectionString},{dataProtectionKeysDatabase}");
                var redis = ConnectionMultiplexer
                    .Connect(configurationOptions);

                services.AddDataProtection()
                    .SetApplicationName("das-admin-service-web")
                    .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");
            }
        }
    }
}
