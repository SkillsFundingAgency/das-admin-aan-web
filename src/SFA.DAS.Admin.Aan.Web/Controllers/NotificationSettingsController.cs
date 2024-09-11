using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Web.Infrastructure;

namespace SFA.DAS.Admin.Aan.Web.Controllers
{
    [Authorize]
    [Route("notification-settings", Name = RouteNames.NotificationSettings)]

    public class NotificationSettingsController : Controller
    {
        public IActionResult Index()
        {
            return Ok("Notification settings placeholder");
        }
    }
}
