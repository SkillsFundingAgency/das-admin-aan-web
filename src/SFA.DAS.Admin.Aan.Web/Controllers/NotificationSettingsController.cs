using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.NotificationSettings;
using SFA.DAS.Validation.Mvc.Filters;

namespace SFA.DAS.Admin.Aan.Web.Controllers
{
    [Authorize]
    [Route("notification-settings", Name = RouteNames.NotificationSettings)]

    public class NotificationSettingsController(IOuterApiClient outerApiClient, ISessionService sessionService) : Controller
    {
        [HttpGet]
        [ValidateModelStateFilter]
        public async Task<IActionResult> Index()
        {
            var adminMemberId = sessionService.GetMemberId();
            var response = await outerApiClient.GetNotificationSettings(adminMemberId, default);
            var viewModel = (NotificationSettingsViewModel) response;
            return View(viewModel);
        }

        [HttpPost]
        [ValidateModelStateFilter]
        public async Task<IActionResult> Index(NotificationSettingsPostRequest request)
        {
            if (!ModelState.IsValid)
            {
                return Ok($"Invalid");
            }

            return Ok($"You have selected: {request.ReceiveNotifications}");
        }
    }
}
