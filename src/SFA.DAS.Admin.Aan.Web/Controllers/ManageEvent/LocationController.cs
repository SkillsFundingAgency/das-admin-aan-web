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
public class LocationController : Controller
{
    public const string ViewPath = "~/Views/ManageEvent/Location.cshtml";
    private readonly ISessionService _sessionService;
    private readonly IValidator<LocationViewModel> _validator;

    public LocationController(ISessionService sessionService, IValidator<LocationViewModel> validator)
    {
        _sessionService = sessionService;
        _validator = validator;
    }

    [HttpGet]
    [Route("events/new/location", Name = RouteNames.ManageEvent.Location)]
    [Route("events/{calendarEventId}/location", Name = RouteNames.UpdateEvent.UpdateLocation)]
    public IActionResult Get()
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();
        var model = GetViewModel(sessionModel);
        return View(ViewPath, model);
    }

    [HttpPost]
    [Route("events/new/location", Name = RouteNames.ManageEvent.Location)]
    [Route("events/{calendarEventId}/location", Name = RouteNames.UpdateEvent.UpdateLocation)]
    public IActionResult Post(LocationViewModel submitModel)
    {
        var result = _validator.Validate(submitModel);

        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            return View(ViewPath, submitModel);
        }

        var sessionModel = _sessionService.Get<EventSessionModel>();

        sessionModel.Location = submitModel.EventLocation?.Trim();
        sessionModel.Latitude = submitModel.Latitude;
        sessionModel.Longitude = submitModel.Longitude;
        sessionModel.Postcode = submitModel.Postcode?.Trim();

        sessionModel.EventLink = submitModel.OnlineEventLink?.Trim();

        _sessionService.Set(sessionModel);

        if (sessionModel.IsAlreadyPublished)
        {
            return RedirectToRoute(RouteNames.CalendarEvent, new { sessionModel.CalendarEventId });
        }

        return sessionModel.HasSeenPreview ?
            RedirectToRoute(RouteNames.ManageEvent.CheckYourAnswers) :
            RedirectToRoute(submitModel.ShowLocationDropdown
                ? RouteNames.ManageEvent.IsAtSchool
                : RouteNames.ManageEvent.OrganiserDetails);
    }

    private LocationViewModel GetViewModel(EventSessionModel sessionModel)
    {
        var pageTitle = sessionModel.IsAlreadyPublished ? UpdateEvent.PageTitle : CreateEvent.PageTitle;

        var cancelLink = Url.RouteUrl(RouteNames.NetworkEvents);

        if (sessionModel!.HasSeenPreview)
        {
            cancelLink = sessionModel.IsAlreadyPublished
                ? Url.RouteUrl(RouteNames.CalendarEvent, new { sessionModel.CalendarEventId })
                : Url.RouteUrl(RouteNames.ManageEvent.CheckYourAnswers);
        }

        var postLink = sessionModel.IsAlreadyPublished
            ? Url.RouteUrl(RouteNames.UpdateEvent.UpdateLocation, new { sessionModel.CalendarEventId })
            : Url.RouteUrl(RouteNames.ManageEvent.Location)!;


        if (sessionModel.HasSeenPreview)
        {
            cancelLink = Url.RouteUrl(RouteNames.ManageEvent.CheckYourAnswers)!;
        }

        return new LocationViewModel
        {
            EventFormat = sessionModel.EventFormat,
            SearchResult = sessionModel.Location,
            OnlineEventLink = sessionModel.EventLink,
            CancelLink = cancelLink,
            PostLink = postLink,
            PageTitle = pageTitle
        };
    }
}