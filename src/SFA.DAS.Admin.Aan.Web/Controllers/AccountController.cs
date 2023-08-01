using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Web.Authentication;

namespace SFA.DAS.Admin.Aan.Web.Controllers;
public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;

    public AccountController(ILogger<AccountController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult SignIn()
    {
        _logger.LogInformation("Start of Sign In");
        var redirectUrl = Url.Action("PostSignIn", "Account");
        return Challenge(
            new AuthenticationProperties { RedirectUri = redirectUrl },
            WsFederationDefaults.AuthenticationScheme);
    }

    [HttpGet]
    public IActionResult PostSignIn()
    {
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

        return SignOut(
            new AuthenticationProperties { RedirectUri = callbackUrl },
            CookieAuthenticationDefaults.AuthenticationScheme,
            WsFederationDefaults.AuthenticationScheme);
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
