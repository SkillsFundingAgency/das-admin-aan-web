using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Web.Authentication;
using SFA.DAS.Admin.Aan.Web.Extensions;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using SFA.DAS.Admin.Aan.Web.Configuration;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace SFA.DAS.Admin.Aan.Web.Controllers;

public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly ApplicationConfiguration _applicationConfiguration;

    public AccountController(ILogger<AccountController> logger, IOptions<ApplicationConfiguration> applicationConfiguration)
    {
        _logger = logger;
        _applicationConfiguration = applicationConfiguration.Value;
    }

    [HttpGet]
    public IActionResult SignIn()
    {
        _logger.LogInformation("Start of Sign In");
        var redirectUrl = Url.Action("PostSignIn", "Account");

        // Get the AuthScheme based on the DfeSignIn config/property.
        var authScheme = _applicationConfiguration.UseDfESignIn
            ? OpenIdConnectDefaults.AuthenticationScheme
            : WsFederationDefaults.AuthenticationScheme;

        return Challenge(
            new AuthenticationProperties { RedirectUri = redirectUrl },
            authScheme);
    }

    [HttpGet]
    public IActionResult PostSignIn()
    {
        if (!User.HasValidRole())
        {
            return RedirectToAction("AccessDenied");
        }
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult SignOut()
    {
        var callbackUrl = Url.Action("SignedOut", "Account", values: null, protocol: Request.Scheme);

        foreach (var cookie in Request.Cookies.Keys)
        {
            Response.Cookies.Delete(cookie);
        }

        // Get the AuthScheme based on the DfeSignIn config/property.
        var authScheme = _applicationConfiguration.UseDfESignIn
            ? OpenIdConnectDefaults.AuthenticationScheme
            : WsFederationDefaults.AuthenticationScheme;

        return SignOut(
            new AuthenticationProperties { RedirectUri = callbackUrl },
            CookieAuthenticationDefaults.AuthenticationScheme,
            authScheme);
    }

    [HttpGet]
    public IActionResult SignedOut()
    {
        return View("SignedOut");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        if (HttpContext.User != null)
        {
            var userName = HttpContext.User.Identity!.Name ?? HttpContext.User.FindFirstValue(ClaimTypes.Upn);
            var roles = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Role || c.Type == Roles.RoleClaimType).Select(c => c.Value);

            _logger.LogError("AccessDenied - User '{userName}' does not have a valid role. They have the following roles: {roles}", userName, string.Join(",", roles));
        }

        return View("AccessDenied");
    }
}
