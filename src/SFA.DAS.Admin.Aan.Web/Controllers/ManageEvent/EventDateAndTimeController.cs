using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Authentication;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.Controllers.ManageEvent;


[Authorize(Roles = Roles.ManageEventsRole)]
public class EventDateAndTimeController : Controller
{
    private readonly ISessionService _sessionService;
    private readonly IValidator<EventDateAndTimeViewModel> _validator;

    public const string ViewPath = "~/Views/ManageEvent/EventDateAndTime.cshtml";
    public EventDateAndTimeController(ISessionService sessionService, IValidator<EventDateAndTimeViewModel> validator)
    {
        _sessionService = sessionService;
        _validator = validator;
    }

    [Authorize(Roles = Roles.ManageEventsRole)]
    [HttpGet]
    [Route("events/new/dateandtime", Name = RouteNames.CreateEvent.DateAndTime)]
    [Route("events/{calendarEventId}/dateandtime", Name = RouteNames.UpdateEvent.UpdateDateAndTime)]
    public IActionResult Get()
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();
        var model = GetViewModel(sessionModel);
        return View(ViewPath, model);
    }

    [HttpPost]
    [Route("events/new/dateandtime", Name = RouteNames.CreateEvent.DateAndTime)]
    [Route("events/{calendarEventId}/dateandtime", Name = RouteNames.UpdateEvent.UpdateDateAndTime)]
    public IActionResult Post(EventDateAndTimeViewModel submitModel)
    {
        var result = _validator.Validate(submitModel);

        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            return View(ViewPath, submitModel);
        }

        var sessionModel = _sessionService.Get<EventSessionModel>();
        sessionModel.DateOfEvent = submitModel.DateOfEvent;

        {
            sessionModel.StartDate = new DateTime(sessionModel.DateOfEvent.Value.Year,
                    sessionModel.DateOfEvent.Value.Month,
                    sessionModel.DateOfEvent.Value.Day, submitModel.StartHour!.Value,
                    submitModel.StartMinutes!.Value, 0, DateTimeKind.Local).ToUniversalTime();

            sessionModel.EndDate = new DateTime(sessionModel.DateOfEvent.Value.Year,
                sessionModel.DateOfEvent.Value.Month,
                sessionModel.DateOfEvent.Value.Day, submitModel.EndHour!.Value,
                submitModel.EndMinutes!.Value, 0, DateTimeKind.Local).ToUniversalTime();
        }

        if (sessionModel.IsAlreadyPublished)
        {
            sessionModel.HasChangedEvent = true;
        }

        _sessionService.Set(sessionModel);

        if (sessionModel.IsAlreadyPublished)
        {
            return RedirectToRoute(RouteNames.CalendarEvent, new { sessionModel.CalendarEventId });
        }

        if (sessionModel.HasSeenPreview)
        {
            return RedirectToRoute(RouteNames.CreateEvent.CheckYourAnswers);
        }

        return RedirectToRoute(RouteNames.CreateEvent.Location);
    }

    private EventDateAndTimeViewModel GetViewModel(EventSessionModel sessionModel)
    {
        string cancelLink;
        string postLink;

        if (sessionModel.IsAlreadyPublished)
        {
            cancelLink = Url.RouteUrl(RouteNames.CalendarEvent, new { sessionModel.CalendarEventId })!;
            postLink = Url.RouteUrl(RouteNames.UpdateEvent.UpdateDateAndTime, new { sessionModel.CalendarEventId })!;
        }
        else
        {
            cancelLink = Url.RouteUrl(sessionModel!.HasSeenPreview
                ? RouteNames.CreateEvent.CheckYourAnswers
                : RouteNames.NetworkEvents)!;

            postLink = Url.RouteUrl(RouteNames.CreateEvent.DateAndTime)!;
        }

        return new EventDateAndTimeViewModel
        {
            DateOfEvent = sessionModel.DateOfEvent,
            StartHour = sessionModel.StartDate?.ToLocalTime().Hour,
            StartMinutes = sessionModel.StartDate?.Minute,
            EndHour = sessionModel.EndDate?.ToLocalTime().Hour,
            EndMinutes = sessionModel.EndDate?.Minute,
            CancelLink = cancelLink,
            PostLink = postLink,
            PageTitle = sessionModel.PageTitle
        };
    }
}
