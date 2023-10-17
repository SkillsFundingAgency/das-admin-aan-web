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
[Route("events/new/location", Name = RouteNames.ManageEvent.Location)]
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
    public IActionResult Get()
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();
        var model = GetViewModel(sessionModel);
        return View(ViewPath, model);
    }

    [HttpPost]
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

        if (sessionModel.HasSeenPreview)
        {
            return RedirectToRoute(RouteNames.ManageEvent.CheckYourAnswers);
        }

        return RedirectToRoute(submitModel.ShowLocationDropdown ? RouteNames.ManageEvent.IsAtSchool : RouteNames.ManageEvent.OrganiserDetails);
    }

    private LocationViewModel GetViewModel(EventSessionModel sessionModel)
    {
        var cancelLink = Url.RouteUrl(RouteNames.NetworkEvents)!;

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
            PostLink = Url.RouteUrl(RouteNames.ManageEvent.Location)!,
            PageTitle = Application.Constants.CreateEvent.PageTitle
        };
    }
}