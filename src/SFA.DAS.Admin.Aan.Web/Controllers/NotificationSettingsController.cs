using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Application.OuterApi.NotificationSettings;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Extensions;
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
        public async Task<IActionResult> Index()
        {
            var adminMemberId = sessionService.GetMemberId();
            var response = await outerApiClient.GetNotificationSettings(adminMemberId, default);
            var viewModel = (NotificationSettingsViewModel) response;
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(NotificationSettingsPostRequest request)
        {
            var adminMemberId = sessionService.GetMemberId();
            var postRequest = new PostNotificationSettings
            {
                ReceiveNotifications = request.ReceiveNotifications!.Value
            };

            await outerApiClient.PostNotificationSettings(adminMemberId, postRequest, default);

            TempData.AddFlashMessage("Notification settings saved", TempDataDictionaryExtensions.FlashMessageLevel.Success);

            return RedirectToRoute(RouteNames.AdministratorHub);
        }
    }
}
