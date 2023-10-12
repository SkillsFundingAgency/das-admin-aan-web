using Microsoft.Extensions.Diagnostics.HealthChecks;
using SFA.DAS.Admin.Aan.Application.Services;

namespace SFA.DAS.Admin.Aan.Web.HealthCheck;

public class AdminAanOuterApiHealthCheck : IHealthCheck
{
    public const string HealthCheckResultDescription = "Admin Aan Outer API Health Check";

    private readonly IOuterApiClient _outerApiClient;
    private readonly ILogger<AdminAanOuterApiHealthCheck> _logger;

    public AdminAanOuterApiHealthCheck(ILogger<AdminAanOuterApiHealthCheck> logger, IOuterApiClient outerApiClient)
    {
        _logger = logger;
        _outerApiClient = outerApiClient;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        _logger.LogInformation("Admin Aan Outer API pinging call");
        try
        {
            await _outerApiClient.GetCalendars(new CancellationToken());
            return HealthCheckResult.Healthy(HealthCheckResultDescription);

        }
        catch (Exception)
        {
            _logger.LogError("Apprentice Aan Outer API ping failed");
            return HealthCheckResult.Unhealthy(HealthCheckResultDescription);
        }
    }
}