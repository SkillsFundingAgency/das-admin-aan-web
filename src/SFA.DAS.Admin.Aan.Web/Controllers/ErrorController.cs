using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.Aan.SharedUi.Models;
using SFA.DAS.Admin.Aan.Web.Authentication;
using SFA.DAS.Admin.Aan.Web.Configuration;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.Account;

namespace SFA.DAS.Admin.Aan.Web.Controllers;

[Authorize(Roles = Roles.ManageEventsRole)]
public class ErrorController : Controller
{
    private readonly ILogger<ErrorController> _logger;
    private readonly IConfiguration _configuration;
    private readonly ApplicationConfiguration _applicationConfiguration;

    public ErrorController(ILogger<ErrorController> logger, IConfiguration configuration, IOptions<ApplicationConfiguration> applicationConfiguration)
    {
        _logger = logger;
        _configuration = configuration;
        _applicationConfiguration = applicationConfiguration.Value;
    }

    [AllowAnonymous]
    [Route("Error/{statuscode}")]
    public IActionResult HttpStatusCodeHandler(int statusCode)
    {
        switch (statusCode)
        {
            case 403:
                if (HttpContext.User != null)
                {
                    var userName = HttpContext.User.Identity!.Name ?? HttpContext.User.FindFirstValue(ClaimTypes.Upn);
                    var roles = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Role || c.Type == Roles.RoleClaimType).Select(c => c.Value);

                    _logger.LogError("AccessDenied - User '{userName}' does not have a valid role. They have the following roles: {roles}", userName, string.Join(",", roles));
                }

                return View("~/Views/Account/AccessDenied.cshtml", new Error403ViewModel(_configuration["ResourceEnvironmentName"] ?? string.Empty) { UseDfESignIn = _applicationConfiguration.UseDfESignIn });
            case 404:
                return View("PageNotFound");
            default:
                ErrorViewModel errorViewModel = new()
                {
                    HomePageUrl = Url.RouteUrl(RouteNames.AdministratorHub)!
                };
                return View("ErrorInService", errorViewModel);
        }
    }

    [AllowAnonymous]
    [Route("Error")]
    public IActionResult ErrorInService()
    {
        var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        ErrorViewModel errorViewModel = new()
        {
            HomePageUrl = Url.RouteUrl(RouteNames.AdministratorHub)!
        };

        if (User.Identity!.IsAuthenticated)
        {
            _logger.LogError(feature!.Error, "Unexpected error occured during request to path: {path} by user: {user}", feature.Path, HttpContext.User.FindFirstValue(ClaimTypes.Upn));
        }
        else
        {
            _logger.LogError(feature!.Error, "Unexpected error occured during request to {path}", feature.Path);
        }
        return View(errorViewModel);
    }
}
