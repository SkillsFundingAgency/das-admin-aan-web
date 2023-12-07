using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Authentication;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.Controllers.ManageEvent;

[Authorize(Roles = Roles.ManageEventsRole)]
public class EventUpdatedController : Controller
{
    private readonly ISessionService _sessionService;

    public EventUpdatedController(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    public const string ViewPath = "~/Views/ManageEvent/EventUpdated.cshtml";

    [HttpGet]
    [Route("events/{calendarEventId}/updated", Name = RouteNames.UpdateEventConfirmation)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public IActionResult Get(Guid calendarEventId)
    {
        _sessionService.Delete(nameof(EventSessionModel));

        var model = new EventUpdatedViewModel
        {
            ManageEventsLink = Url.RouteUrl(RouteNames.NetworkEvents)!,
            AdministratorHubLink = Url.RouteUrl(RouteNames.AdministratorHub)
        };

        return View(ViewPath, model);
    }
}