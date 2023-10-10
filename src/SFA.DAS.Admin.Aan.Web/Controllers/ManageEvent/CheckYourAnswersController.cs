using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Application.Constants;
using SFA.DAS.Admin.Aan.Application.OuterApi.CalendarEvents;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Authentication;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.Controllers.ManageEvent;

[Authorize(Roles = Roles.ManageEventsRole)]
[Route("events/new/check-your-answers", Name = RouteNames.ManageEvent.CheckYourAnswers)]
public class CheckYourAnswersController : Controller
{
    private readonly IOuterApiClient _outerApiClient;
    private readonly ISessionService _sessionService;

    public const string ViewPath = "~/Views/ManageEvent/CheckYourAnswers.cshtml";

    public CheckYourAnswersController(ISessionService sessionService, IOuterApiClient outerApiClient)
    {
        _sessionService = sessionService;
        _outerApiClient = outerApiClient;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();
        var model = await GetViewModel(sessionModel, cancellationToken);
        return View(ViewPath, model);
    }

    [HttpPost]
    public async Task<IActionResult> Post(CancellationToken cancellationToken)
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();

        var request = (CreateEventRequest)sessionModel;

        var calendarEventResponse = await _outerApiClient.PostCalendarEvent(_sessionService.GetMemberId(), request, cancellationToken);

        _sessionService.Delete(nameof(EventSessionModel));

        return RedirectToRoute(RouteNames.ManageEvent.EventPublished, new { eventId = calendarEventResponse.CalendarEventId });
    }

    private async Task<CheckYourAnswersViewModel> GetViewModel(EventSessionModel sessionModel, CancellationToken cancellationToken)
    {
        var calendarTask = _outerApiClient.GetCalendars(cancellationToken);
        var regionTask = _outerApiClient.GetRegions(cancellationToken);

        List<Task> tasks = new() { calendarTask, regionTask };
        await Task.WhenAll(tasks);

        var eventTypes = calendarTask.Result;
        var regions = regionTask.Result.Regions.Select(reg => new RegionSelection(reg.Area, reg.Id)).ToList();
        regions.Add(new RegionSelection("National", 0));

        var model = (CheckYourAnswersViewModel)sessionModel;
        model.HasSeenPreview = true;
        model.PageTitle = CreateEvent.PageTitle;
        model.CancelLink = Url.RouteUrl(RouteNames.NetworkEvents)!;
        model.PostLink = "#";
        model.PreviewLink = "#";
        model.EventType = eventTypes.First(x => x.Id == sessionModel.CalendarId).CalendarName;
        model.EventRegion = regions.First(x => x.RegionId == sessionModel.RegionId).Name;

        return model;
    }
}