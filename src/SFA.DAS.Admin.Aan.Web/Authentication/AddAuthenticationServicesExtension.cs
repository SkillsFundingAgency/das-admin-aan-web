using System.Diagnostics.CodeAnalysis;
using SFA.DAS.Admin.Aan.Web.Configuration;
using SFA.DAS.DfESignIn.Auth.AppStart;

namespace SFA.DAS.Admin.Aan.Web.Authentication;

[ExcludeFromCodeCoverage]
public static class AddAuthenticationServicesExtension
{
    private const string CookieAuthName = "SFA.DAS.AdminService.Web.Auth";

    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
    {
        var applicationConfiguration = configuration.GetSection(nameof(ApplicationConfiguration)).Get<ApplicationConfiguration>();

        services.AddAndConfigureDfESignInAuthentication(
            configuration,
            CookieAuthName,
            typeof(CustomServiceRole),
            DfESignIn.Auth.Enums.ClientName.ServiceAdminAan,
            "/SignOut",
            "");

        return services;
    }
}
