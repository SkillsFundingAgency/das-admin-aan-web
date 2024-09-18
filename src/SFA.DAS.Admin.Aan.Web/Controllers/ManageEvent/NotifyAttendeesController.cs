using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Application.OuterApi.CalendarEvents;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Authentication;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.Controllers.ManageEvent;

[Authorize(Roles = Roles.ManageEventsRole)]
public class NotifyAttendeesController : Controller
{
    public const string ViewPath = "~/Views/ManageEvent/NotifyAttendees.cshtml";
    private readonly IOuterApiClient _outerApiClient;
    private readonly ISessionService _sessionService;
    private readonly IValidator<NotifyAttendeesViewModel> _validator;

    public NotifyAttendeesController(IOuterApiClient outerApiClient, ISessionService sessionService, IValidator<NotifyAttendeesViewModel> validator)
    {
        _outerApiClient = outerApiClient;
        _sessionService = sessionService;
        _validator = validator;
    }


    [HttpGet]
    [Route("events/{calendarEventId}/notify-attendees", Name = RouteNames.UpdateEvent.NotifyAttendees)]
    public IActionResult Get(Guid calendarEventId)
    {
        var model = new NotifyAttendeesViewModel
        {
            CancelLink = Url.RouteUrl(RouteNames.NetworkEvents)!,
            PostLink = Url.RouteUrl(RouteNames.UpdateEvent.NotifyAttendees, new { calendarEventId })!
        };

        return View(ViewPath, model);
    }

    [HttpPost]
    [Route("events/{calendarEventId}/notify-attendees", Name = RouteNames.UpdateEvent.NotifyAttendees)]
    public async Task<IActionResult> Post(NotifyAttendeesViewModel submitModel, Guid calendarEventId, CancellationToken cancellationToken)
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();

        var request = (UpdateCalendarEventRequest)sessionModel;
        request.SendUpdateEventNotification = submitModel.IsNotifyAttendees ?? false;

        await _outerApiClient.UpdateCalendarEvent(_sessionService.GetMemberId(), calendarEventId, request, cancellationToken);

        return RedirectToRoute(RouteNames.UpdateEventConfirmation, new { calendarEventId = sessionModel.CalendarEventId });
    }
}
