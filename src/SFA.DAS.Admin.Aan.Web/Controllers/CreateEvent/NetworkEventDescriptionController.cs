using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

namespace SFA.DAS.Admin.Aan.Web.Controllers.CreateEvent;

[Authorize]
[Route("manage-events/new/event-description", Name = RouteNames.CreateEvent.EventDescription)]
public class NetworkEventDescriptionController : Controller
{
    private readonly IOuterApiClient _outerApiClient;
    private readonly ISessionService _sessionService;
    private readonly IValidator<CreateEventDescriptionViewModel> _validator;

    public const string ViewPath = "~/Views/NetworkEvent/EventDescription.cshtml";
    public NetworkEventDescriptionController(IOuterApiClient outerApiClient, ISessionService sessionService, IValidator<CreateEventDescriptionViewModel> validator)
    {
        _outerApiClient = outerApiClient;
        _sessionService = sessionService;
        _validator = validator;
    }
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var sessionModel = _sessionService.Get<CreateEventSessionModel>();
        var model = await GetViewModel(sessionModel, cancellationToken);
        return View(ViewPath, model);
    }

    [HttpPost]
    public async Task<IActionResult> Post(CreateEventDescriptionViewModel submitModel, CancellationToken cancellationToken)
    {
        var sessionModel = _sessionService.Get<CreateEventSessionModel?>();
        if (sessionModel == null) return RedirectToAction("Get", "NetworkEventFormat");

        sessionModel.EventOutline = submitModel.EventOutline;
        sessionModel.EventSummary = submitModel.EventSummary;
        sessionModel.GuestSpeaker = submitModel.GuestSpeaker;

        var result = await _validator.ValidateAsync(submitModel, cancellationToken);

        if (!result.IsValid)
        {

            result.AddToModelState(ModelState);
            return View(ViewPath, await GetViewModel(sessionModel, cancellationToken));
        }

        _sessionService.Set(sessionModel);
        return RedirectToAction("Get", "NetworkEventDescription");
    }

    private async Task<CreateEventDescriptionViewModel> GetViewModel(CreateEventSessionModel sessionModel, CancellationToken cancellationToken)
    {
        return new CreateEventDescriptionViewModel
        {
            EventOutline = sessionModel?.EventOutline,
            EventSummary = sessionModel?.EventSummary,
            GuestSpeaker = sessionModel?.GuestSpeaker,
            BackLink = Url.RouteUrl(RouteNames.NetworkEvents)!
        };
    }
}
