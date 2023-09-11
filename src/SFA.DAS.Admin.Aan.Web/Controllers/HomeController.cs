using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Web.Authentication;
using SFA.DAS.Admin.Aan.Web.Infrastructure;

namespace SFA.DAS.Admin.Aan.Web.Controllers;

[Authorize(Roles = $"{Roles.ManageMembersRole}, {Roles.ManageEventsRole}")]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return RedirectToRoute(RouteNames.AdministratorHub);
    }
}
