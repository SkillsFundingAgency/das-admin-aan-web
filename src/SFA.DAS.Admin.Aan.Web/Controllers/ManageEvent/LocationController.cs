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
    [Route("events/new/location", Name = RouteNames.CreateEvent.Location)]
    [Route("events/{calendarEventId}/location", Name = RouteNames.UpdateEvent.UpdateLocation)]
    public IActionResult Get()
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();
        var model = GetViewModel(sessionModel);
        return View(ViewPath, model);
    }

    [HttpPost]
    [Route("events/new/location", Name = RouteNames.CreateEvent.Location)]
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

        return RedirectToRoute(submitModel.ShowLocationDropdown
                ? RouteNames.CreateEvent.IsAtSchool
                : RouteNames.CreateEvent.OrganiserDetails);
    }

    private LocationViewModel GetViewModel(EventSessionModel sessionModel)
    {
        string cancelLink;
        string postLink;

        if (sessionModel.IsAlreadyPublished)
        {
            cancelLink = Url.RouteUrl(RouteNames.CalendarEvent, new { sessionModel.CalendarEventId })!;
            postLink = Url.RouteUrl(RouteNames.UpdateEvent.UpdateLocation, new { sessionModel.CalendarEventId })!;
        }
        else
        {
            cancelLink = Url.RouteUrl(sessionModel!.HasSeenPreview
                ? RouteNames.CreateEvent.CheckYourAnswers
                : RouteNames.NetworkEvents)!;

            postLink = Url.RouteUrl(RouteNames.CreateEvent.Location)!;
        }

        return new LocationViewModel
        {
            EventFormat = sessionModel.EventFormat,
            SearchResult = sessionModel.Location,
            OnlineEventLink = sessionModel.EventLink,
            CancelLink = cancelLink,
            PostLink = postLink,
            PageTitle = sessionModel.PageTitle
        };
    }
}