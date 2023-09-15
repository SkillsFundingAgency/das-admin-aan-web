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
[Route("events/new/description", Name = RouteNames.ManageEvent.EventDescription)]
public class NetworkEventDescriptionController : Controller
{
    private readonly ISessionService _sessionService;
    private readonly IValidator<EventDescriptionViewModel> _validator;

    public const string ViewPath = "~/Views/NetworkEvent/EventDescription.cshtml";
    public NetworkEventDescriptionController(ISessionService sessionService, IValidator<EventDescriptionViewModel> validator)
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
    public IActionResult Post(EventDescriptionViewModel submitModel)
    {
        var sessionModel = _sessionService.Get<EventSessionModel?>();
        sessionModel!.EventOutline = submitModel.EventOutline;
        sessionModel.EventSummary = submitModel.EventSummary;

        var result = _validator.Validate(submitModel);

        if (!result.IsValid)
        {

            result.AddToModelState(ModelState);
            return View(ViewPath, GetViewModel(sessionModel));
        }

        _sessionService.Set(sessionModel);

        return RedirectToRoute(RouteNames.ManageEvent.EventHasGuestSpeakers);
    }

    private EventDescriptionViewModel GetViewModel(EventSessionModel sessionModel)
    {
        return new EventDescriptionViewModel
        {
            EventOutline = sessionModel?.EventOutline,
            EventSummary = sessionModel?.EventSummary,
            CancelLink = Url.RouteUrl(RouteNames.NetworkEvents)!,
            PostLink = Url.RouteUrl(RouteNames.ManageEvent.EventFormat)!,
            PageTitle = Application.Constants.CreateEvent.PageTitle
        };
    }
}
