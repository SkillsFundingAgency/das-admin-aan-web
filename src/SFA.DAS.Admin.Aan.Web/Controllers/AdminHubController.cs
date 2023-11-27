using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Aan.SharedUi.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Authentication;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models;

namespace SFA.DAS.Admin.Aan.Web.Controllers;

[Authorize]
[Route("admin-hub", Name = RouteNames.AdministratorHub)]
public class AdminHubController : Controller
{
    public IActionResult Index()
    {
        return View(new AdminHubViewModel(User.IsInRole(Roles.ManageEventsRole), User.IsInRole(Roles.ManageMembersRole), Url.RouteUrl(RouteNames.NetworkEvents)!, Url.RouteUrl(SharedRouteNames.NetworkDirectory)!));
    }
}
