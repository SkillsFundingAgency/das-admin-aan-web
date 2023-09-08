using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

namespace SFA.DAS.Admin.Aan.Web.Controllers.CreateEvent;

[Authorize]
[Route("manage-events/new/event-guest-speaker", Name = RouteNames.CreateEvent.EventGuestSpeaker)]
public class EventGuestSpeakerController : Controller
{
    private readonly ISessionService _sessionService;
    private readonly IValidator<CreateEventGuestSpeakerViewModel> _validator;

    public const string ViewPath = "~/Views/NetworkEvent/EventGuestSpeaker.cshtml";
    public EventGuestSpeakerController(ISessionService sessionService, IValidator<CreateEventGuestSpeakerViewModel> validator)
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
    public IActionResult Post(CreateEventGuestSpeakerViewModel submitModel)
    {
        var sessionModel = _sessionService.Get<CreateEventSessionModel?>();
        if (sessionModel == null) return RedirectToAction("Get", "NetworkEventFormat");

        sessionModel.GuestSpeaker = submitModel.GuestSpeaker;

        var result = _validator.Validate(submitModel);

        if (!result.IsValid)
        {

            result.AddToModelState(ModelState);
            return View(ViewPath, GetViewModel(sessionModel));
        }

        _sessionService.Set(sessionModel);
        return RedirectToAction("Get", "EventGuestSpeaker");
    }

    private CreateEventGuestSpeakerViewModel GetViewModel(CreateEventSessionModel sessionModel)
    {
        return new CreateEventGuestSpeakerViewModel
        {
            GuestSpeaker = sessionModel?.GuestSpeaker,
            CancelLink = Url.RouteUrl(RouteNames.NetworkEvents)!,
            PostLink = Url.RouteUrl(RouteNames.CreateEvent.EventFormat)!,
            PageTitle = Application.Constants.CreateEvent.PageTitle
        };
    }
}
