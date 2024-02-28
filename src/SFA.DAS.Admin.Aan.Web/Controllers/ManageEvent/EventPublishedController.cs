using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Web.Authentication;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.Controllers.ManageEvent;

[Authorize(Roles = Roles.ManageEventsRole)]
public class EventPublishedController : Controller
{
    public const string ViewPath = "~/Views/ManageEvent/EventPublished.cshtml";

    [HttpGet]
    [Route("events/{eventId}/published", Name = RouteNames.CreateEvent.EventPublished)]
    public IActionResult Get()
    {
        var model = new EventPublishedViewModel
        {
            ManageEventsLink = Url.RouteUrl(RouteNames.NetworkEvents)!
        };

        return View(ViewPath, model);
    }
}
