using Microsoft.Extensions.Diagnostics.HealthChecks;
using SFA.DAS.Admin.Aan.Application.Services;

namespace SFA.DAS.Admin.Aan.Web.HealthCheck;

public class AdminAanOuterApiHealthCheck(ILogger<AdminAanOuterApiHealthCheck> logger, IOuterApiClient outerApiClient)
    : IHealthCheck
{
    public const string HealthCheckResultDescription = "Outer API Health Check";

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        logger.LogInformation("Admin Aan Outer API pinging call");
        try
        {
            await outerApiClient.GetCalendars(new CancellationToken());
            return HealthCheckResult.Healthy(HealthCheckResultDescription);

        }
        catch (Exception)
        {
            logger.LogError("Apprentice Aan Outer API ping failed");
            return HealthCheckResult.Unhealthy(HealthCheckResultDescription);
        }
    }
}