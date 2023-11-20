using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Web.Authentication;

namespace SFA.DAS.Admin.Aan.Web.Controllers.ManageMembers;

[Authorize(Roles = Roles.ManageMembersRole)]
public class ManageMembersController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
