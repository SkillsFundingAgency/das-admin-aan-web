using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Authentication;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

namespace SFA.DAS.Admin.Aan.Web.Controllers.CreateEvent;

[Authorize(Roles = Roles.ManageEventsRole)]
[Route("manage-events/new/event-guest-speaker", Name = RouteNames.CreateEvent.EventHasGuestSpeakers)]
public class EventHasGuestSpeakersController : Controller
{
    private readonly ISessionService _sessionService;
    private readonly IValidator<CreateEventHasGuestSpeakersViewModel> _validator;

    public const string ViewPath = "~/Views/NetworkEvent/EventGuestSpeaker.cshtml";
    public EventHasGuestSpeakersController(ISessionService sessionService, IValidator<CreateEventHasGuestSpeakersViewModel> validator)
    {
        _sessionService = sessionService;
        _validator = validator;
    }
    [HttpGet]
    public IActionResult Get()
    {
        var sessionModel = _sessionService.Get<CreateEventSessionModel>();
        var model = GetViewModel(sessionModel);
        return View(ViewPath, model);
    }

    [HttpPost]
    public IActionResult Post(CreateEventHasGuestSpeakersViewModel submitModel)
    {
        var sessionModel = _sessionService.Get<CreateEventSessionModel?>();
        if (sessionModel == null) return RedirectToAction("Get", "NetworkEventFormat");

        sessionModel.HasGuestSpeakers = submitModel.HasGuestSpeakers;

        var result = _validator.Validate(submitModel);

        if (!result.IsValid)
        {

            result.AddToModelState(ModelState);
            return View(ViewPath, GetViewModel(sessionModel));
        }

        _sessionService.Set(sessionModel);

        if (sessionModel.HasGuestSpeakers == true) return RedirectToAction("Get", "GuestSpeakerList");
        return RedirectToAction("Get", "EventHasGuestSpeakers");
    }

    private CreateEventHasGuestSpeakersViewModel GetViewModel(CreateEventSessionModel sessionModel)
    {
        return new CreateEventHasGuestSpeakersViewModel
        {
            HasGuestSpeakers = sessionModel?.HasGuestSpeakers,
            CancelLink = Url.RouteUrl(RouteNames.NetworkEvents)!,
            PostLink = Url.RouteUrl(RouteNames.CreateEvent.EventHasGuestSpeakers)!,
            PageTitle = Application.Constants.CreateEvent.PageTitle
        };
    }
}
