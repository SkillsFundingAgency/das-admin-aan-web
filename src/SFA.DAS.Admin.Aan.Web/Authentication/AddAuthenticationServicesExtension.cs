using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
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

        if (applicationConfiguration is { UseDfESignIn: true })
        {
            services.AddAndConfigureDfESignInAuthentication(
                configuration,
                CookieAuthName,
                typeof(CustomServiceRole),
                DfESignIn.Auth.Enums.ClientName.ServiceAdmin,
                "/SignOut",
                "");
        }
        else
        {
            var authConfig = configuration.GetSection(nameof(StaffAuthenticationConfiguration))
                .Get<StaffAuthenticationConfiguration>();

            var cookieOptions = new Action<CookieAuthenticationOptions>(options =>
            {
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            services
                .AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultChallengeScheme = WsFederationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultSignOutScheme = WsFederationDefaults.AuthenticationScheme;
                })
                .AddWsFederation(options =>
                {
                    options.Wtrealm = authConfig?.WtRealm;
                    options.MetadataAddress = authConfig?.MetadataAddress;
                    options.TokenValidationParameters.RoleClaimType = Roles.RoleClaimType;
                    options.Events.OnSecurityTokenValidated = async (ctx) =>
                    {
                        await PopulateProviderClaims(ctx.HttpContext, ctx.Principal!);
                    };
                })
                .AddCookie(cookieOptions);
        }

        return services;
    }

    private static Task PopulateProviderClaims(HttpContext httpContext, ClaimsPrincipal principal)
    {
        var userId = principal.Claims.First(c => c.Type.Equals(StaffClaims.UserId)).Value;
        httpContext.Items.Add(StaffClaims.UserId, userId);

        var givenName = principal.Claims.First(c => c.Type.Equals(StaffClaims.GivenName)).Value;
        httpContext.Items.Add(StaffClaims.GivenName, givenName);

        var surname = principal.Claims.First(c => c.Type.Equals(StaffClaims.Surname)).Value;
        httpContext.Items.Add(StaffClaims.Surname, surname);

        principal.Identities.First().AddClaim(new Claim(StaffClaims.GivenName, givenName));
        principal.Identities.First().AddClaim(new Claim(StaffClaims.Surname, surname));
        principal.Identities.First().AddClaim(new Claim(StaffClaims.UserId, userId));
        return Task.CompletedTask;
    }
}
