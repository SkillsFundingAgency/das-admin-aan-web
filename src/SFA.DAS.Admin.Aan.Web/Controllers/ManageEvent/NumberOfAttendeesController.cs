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
public class NumberOfAttendeesController : Controller
{
    private readonly ISessionService _sessionService;
    private readonly IValidator<NumberOfAttendeesViewModel> _validator;

    public const string ViewPath = "~/Views/ManageEvent/NumberOfAttendees.cshtml";
    public NumberOfAttendeesController(ISessionService sessionService, IValidator<NumberOfAttendeesViewModel> validator)
    {
        _sessionService = sessionService;
        _validator = validator;
    }

    [HttpGet]
    [Route("events/new/attendees", Name = RouteNames.CreateEvent.NumberOfAttendees)]
    [Route("events/{calendarEventId}/attendees", Name = RouteNames.UpdateEvent.UpdateNumberOfAttendees)]
    public IActionResult Get()
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();
        var model = GetViewModel(sessionModel);
        return View(ViewPath, model);
    }

    [HttpPost]
    [Route("events/new/attendees", Name = RouteNames.CreateEvent.NumberOfAttendees)]
    [Route("events/{calendarEventId}/attendees", Name = RouteNames.UpdateEvent.UpdateNumberOfAttendees)]
    public IActionResult Post(NumberOfAttendeesViewModel submitModel)
    {
        var result = _validator.Validate(submitModel);


        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            return View(ViewPath, submitModel);
        }

        var sessionModel = _sessionService.Get<EventSessionModel>();
        sessionModel.PlannedAttendees = submitModel.NumberOfAttendees;

        if (sessionModel.IsAlreadyPublished)
        {
            sessionModel.HasChangedEvent = true;
        }

        _sessionService.Set(sessionModel);


        if (sessionModel.IsAlreadyPublished)
        {
            return RedirectToRoute(RouteNames.CalendarEvent, new { sessionModel.CalendarEventId });
        }


        return RedirectToRoute(RouteNames.CreateEvent.CheckYourAnswers);
    }

    private NumberOfAttendeesViewModel GetViewModel(EventSessionModel sessionModel)
    {
        string cancelLink;
        string postLink;

        if (sessionModel.IsAlreadyPublished)
        {
            cancelLink = Url.RouteUrl(RouteNames.CalendarEvent, new { sessionModel.CalendarEventId })!;
            postLink = Url.RouteUrl(RouteNames.UpdateEvent.UpdateNumberOfAttendees, new { sessionModel.CalendarEventId })!;
        }
        else
        {
            cancelLink = Url.RouteUrl(RouteNames.NetworkEvents)!;
            postLink = Url.RouteUrl(RouteNames.CreateEvent.NumberOfAttendees)!;

            if (sessionModel.HasSeenPreview)
            {
                cancelLink = Url.RouteUrl(RouteNames.CreateEvent.CheckYourAnswers)!;
            }
        }

        return new NumberOfAttendeesViewModel
        {
            NumberOfAttendees = sessionModel.PlannedAttendees,
            CancelLink = cancelLink,
            PostLink = postLink,
            PageTitle = sessionModel.PageTitle
        };
    }
}
