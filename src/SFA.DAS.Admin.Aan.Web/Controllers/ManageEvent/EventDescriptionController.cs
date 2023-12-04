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
public class EventDescriptionController : Controller
{
    private readonly ISessionService _sessionService;
    private readonly IValidator<EventDescriptionViewModel> _validator;

    public const string ViewPath = "~/Views/ManageEvent/EventDescription.cshtml";
    public EventDescriptionController(ISessionService sessionService, IValidator<EventDescriptionViewModel> validator)
    {
        _sessionService = sessionService;
        _validator = validator;
    }

    [HttpGet]
    [Route("events/new/description", Name = RouteNames.CreateEvent.Description)]
    [Route("events/{calendarEventId}/description", Name = RouteNames.UpdateEvent.UpdateDescription)]
    public IActionResult Get()
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();
        var model = GetViewModel(sessionModel);
        return View(ViewPath, model);
    }

    [HttpPost]
    [Route("events/new/description", Name = RouteNames.CreateEvent.Description)]
    [Route("events/{calendarEventId}/description", Name = RouteNames.UpdateEvent.UpdateDescription)]
    public IActionResult Post(EventDescriptionViewModel submitModel)
    {

        var result = _validator.Validate(submitModel);

        if (!result.IsValid)
        {

            result.AddToModelState(ModelState);
            return View(ViewPath, submitModel);
        }

        var sessionModel = _sessionService.Get<EventSessionModel>();
        sessionModel.EventOutline = submitModel.EventOutline?.Trim();
        sessionModel.EventSummary = submitModel.EventSummary?.Trim();
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

        return RedirectToRoute(RouteNames.CreateEvent.HasGuestSpeakers);
    }

    private EventDescriptionViewModel GetViewModel(EventSessionModel sessionModel)
    {
        string cancelLink;
        string postLink;

        if (sessionModel.IsAlreadyPublished)
        {
            cancelLink = Url.RouteUrl(RouteNames.CalendarEvent, new { sessionModel.CalendarEventId })!;
            postLink = Url.RouteUrl(RouteNames.UpdateEvent.UpdateDescription, new { sessionModel.CalendarEventId })!;
        }
        else
        {
            cancelLink = Url.RouteUrl(sessionModel!.HasSeenPreview
                ? RouteNames.CreateEvent.CheckYourAnswers
                : RouteNames.NetworkEvents)!;

            postLink = Url.RouteUrl(RouteNames.CreateEvent.Description)!;
        }

        return new EventDescriptionViewModel
        {
            EventOutline = sessionModel.EventOutline?.Trim(),
            EventSummary = sessionModel.EventSummary?.Trim(),
            CancelLink = cancelLink,
            PostLink = postLink,
            PageTitle = sessionModel.PageTitle
        };
    }
}
