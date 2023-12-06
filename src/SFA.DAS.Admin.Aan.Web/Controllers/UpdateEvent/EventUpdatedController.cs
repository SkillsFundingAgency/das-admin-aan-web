using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Web.Authentication;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.Controllers.UpdateEvent;

[Authorize(Roles = Roles.ManageEventsRole)]
public class EventUpdatedController : Controller
{
    public const string ViewPath = "~/Views/ManageEvent/EventUpdated.cshtml";

    [HttpGet]
    [Route("events/{calendarEventId}/updated", Name = RouteNames.UpdateEventConfirmation)]
    public IActionResult Get(Guid calendarEventId)
    {
        var model = new EventUpdatedViewModel
        {
            ManageEventsLink = Url.RouteUrl(RouteNames.NetworkEvents)!,
            AdministratorHubLink = Url.RouteUrl(RouteNames.AdministratorHub)
        };

        return View(ViewPath, model);
    }
}