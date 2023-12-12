using System.Diagnostics.CodeAnalysis;
using RestEase.HttpClientFactory;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Configuration;
using SFA.DAS.Http.Configuration;

namespace SFA.DAS.Admin.Aan.Web.AppStart;

[ExcludeFromCodeCoverage]
public static class AddServiceRegistrationsExtension
{
    public static IServiceCollection AddServiceRegistrations(this IServiceCollection services, IConfigurationRoot configuration)
    {
        var outerApiConfiguration = configuration.GetSection(nameof(AdminAanOuterApiConfiguration)).Get<AdminAanOuterApiConfiguration>()!;
        AddOuterApi(services, outerApiConfiguration);

        services.AddTransient<ISessionService, SessionService>();
        return services;
    }

    private static void AddOuterApi(this IServiceCollection services, AdminAanOuterApiConfiguration configuration)
    {
        services.AddTransient<IApimClientConfiguration>((_) => configuration);

        services.AddScoped<Http.MessageHandlers.DefaultHeadersHandler>();
        services.AddScoped<Http.MessageHandlers.LoggingMessageHandler>();
        services.AddScoped<Http.MessageHandlers.ApimHeadersHandler>();

        services
            .AddRestEaseClient<IOuterApiClient>(configuration.ApiBaseUrl)
            .AddHttpMessageHandler<Http.MessageHandlers.DefaultHeadersHandler>()
            .AddHttpMessageHandler<Http.MessageHandlers.ApimHeadersHandler>()
            .AddHttpMessageHandler<Http.MessageHandlers.LoggingMessageHandler>();
    }
}
