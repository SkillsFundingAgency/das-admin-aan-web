using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Aan.SharedUi.Models;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Authentication;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.Controllers.ManageEvent;

[Authorize(Roles = Roles.ManageEventsRole)]
public class PreviewEventController : Controller
{
    private readonly IOuterApiClient _outerApiClient;
    private readonly ISessionService _sessionService;
    public const string DetailsViewPath = "~/Views/NetworkEventDetails/Detail.cshtml";

    public PreviewEventController(IOuterApiClient outerApiClient, ISessionService sessionService)
    {
        _outerApiClient = outerApiClient;
        _sessionService = sessionService;
    }

    [HttpGet]
    [Route("events/new/preview", Name = RouteNames.ManageEvent.PreviewEvent)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();
        var model = await GetViewModel(sessionModel, cancellationToken);
        return View(DetailsViewPath, model);
    }

    private async Task<NetworkEventDetailsViewModel> GetViewModel(EventSessionModel sessionModel, CancellationToken cancellationToken)
    {
        var calendars = await _outerApiClient.GetCalendars(cancellationToken);
        sessionModel.CalendarName = calendars.First(x => x.Id == sessionModel.CalendarId).CalendarName;
        var model = (NetworkEventDetailsViewModel)sessionModel;

        model.IsPreview = true;
        model.BackLinkUrl = Url.RouteUrl(RouteNames.ManageEvent.CheckYourAnswers)!;

        return model;
    }
}
