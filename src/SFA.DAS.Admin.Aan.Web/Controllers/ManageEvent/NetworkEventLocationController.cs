using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Authentication;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

namespace SFA.DAS.Admin.Aan.Web.Controllers.ManageEvent;

[Authorize(Roles = Roles.ManageEventsRole)]
[Route("events/new/location", Name = RouteNames.ManageEvent.EventLocation)]
public class NetworkEventLocationController : Controller
{
    public const string ViewPath = "~/Views/ManageEvent/EventLocation.cshtml";
    private readonly ISessionService _sessionService;
    private readonly IValidator<EventLocationViewModel> _validator;

    public NetworkEventLocationController(ISessionService sessionService, IValidator<EventLocationViewModel> validator)
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
    public IActionResult Post(EventLocationViewModel submitModel)
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();


        sessionModel.EventLocation = submitModel.EventLocation;
        sessionModel.OnlineEventLink = submitModel.OnlineEventLink;

        var result = _validator.Validate(submitModel);

        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            return View(ViewPath, GetViewModel(sessionModel));
        }

        _sessionService.Set(sessionModel);
        return RedirectToRoute(RouteNames.ManageEvent.EventLocation);
    }

    private EventLocationViewModel GetViewModel(EventSessionModel sessionModel)
    {
        var locationTitle = "In person event location";
        return new EventLocationViewModel
        {
            LocationTitle = locationTitle,
            SearchResult = sessionModel.EventLocation,
            OnlineEventLink = sessionModel.OnlineEventLink,
            CancelLink = Url.RouteUrl(RouteNames.NetworkEvents)!,
            PostLink = Url.RouteUrl(RouteNames.ManageEvent.EventFormat)!,
            PageTitle = Application.Constants.CreateEvent.PageTitle
        };
    }
}
