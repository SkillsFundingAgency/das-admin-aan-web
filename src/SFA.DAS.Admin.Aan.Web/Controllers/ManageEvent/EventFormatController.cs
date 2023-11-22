using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Application.Constants;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Authentication;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.Controllers.ManageEvent;

[Authorize(Roles = Roles.ManageEventsRole)]
public class EventFormatController : Controller
{
    public const string ViewPath = "~/Views/ManageEvent/EventFormat.cshtml";
    private readonly ISessionService _sessionService;
    private readonly IValidator<EventFormatViewModel> _validator;

    public EventFormatController(ISessionService sessionService, IValidator<EventFormatViewModel> validator)
    {
        _sessionService = sessionService;
        _validator = validator;
    }

    [HttpGet]
    [Route("events/new/format", Name = RouteNames.ManageEvent.EventFormat)]
    [Route("events/{calendarEventId}/format", Name = RouteNames.UpdateEvent.UpdateEventFormat)]
    public IActionResult Get()
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();
        var model = GetViewModel(sessionModel);
        return View(ViewPath, model);
    }

    [HttpPost]
    [Route("events/new/format", Name = RouteNames.ManageEvent.EventFormat)]
    [Route("events/{calendarEventId}/format", Name = RouteNames.UpdateEvent.UpdateEventFormat)]
    public IActionResult Post(EventFormatViewModel submitModel)
    {
        var result = _validator.Validate(submitModel);

        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            return View(ViewPath, submitModel);
        }

        var sessionModel = _sessionService.Get<EventSessionModel>();
        sessionModel.EventFormat = submitModel.EventFormat;

        _sessionService.Set(sessionModel);

        if (sessionModel.HasSeenPreview)
        {
            return sessionModel.IsAlreadyPublished
                ? RedirectToRoute(RouteNames.UpdateEvent.UpdateLocation, new { sessionModel.CalendarEventId })
                : RedirectToRoute(RouteNames.ManageEvent.Location);
        }

        if (sessionModel.IsAlreadyPublished)
        {
            return RedirectToRoute(RouteNames.CalendarEvent, new { sessionModel.CalendarEventId });
        }

        return RedirectToRoute(RouteNames.ManageEvent.EventType);
    }

    private EventFormatViewModel GetViewModel(EventSessionModel sessionModel)
    {
        var pageTitle = sessionModel.IsAlreadyPublished ? UpdateEvent.PageTitle : CreateEvent.PageTitle;

        var model = new EventFormatViewModel
        {
            EventFormat = sessionModel.EventFormat,
            CancelLink = Url.RouteUrl(RouteNames.NetworkEvents)!,
            PageTitle = pageTitle
        };

        if (sessionModel!.HasSeenPreview)
        {
            model.CancelLink = sessionModel.IsAlreadyPublished
                ? Url.RouteUrl(RouteNames.CalendarEvent, new { sessionModel.CalendarEventId })
                : Url.RouteUrl(RouteNames.ManageEvent.CheckYourAnswers)!;
        }

        model.PostLink = sessionModel.IsAlreadyPublished
            ? Url.RouteUrl(RouteNames.UpdateEvent.UpdateEventFormat, new { sessionModel.CalendarEventId })
            : Url.RouteUrl(RouteNames.ManageEvent.EventFormat)!;

        return model;
    }
}