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
    [Route("events/new/check-your-answers", Name = RouteNames.ManageEvent.CheckYourAnswers)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();
        var model = await GetViewModel(sessionModel, cancellationToken);
        return View(ViewPath, model);
    }

    [HttpPost]
    [Route("events/new/check-your-answers", Name = RouteNames.ManageEvent.CheckYourAnswers)]
    public async Task<IActionResult> Post(CancellationToken cancellationToken)
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();

        var request = (CreateEventRequest)sessionModel;

        request.RequestedByMemberId = GetMemberId();
        var calendarEventResponse = await _outerApiClient.PostCalendarEvent(request, cancellationToken);

        return RedirectToRoute(RouteNames.ManageEvent.EventPublished, new { eventId = calendarEventResponse.CalendarEventId });
    }

    [HttpGet]
    [Route("events/{eventId}/published", Name = RouteNames.ManageEvent.EventPublished)]
    public IActionResult EventPublished(Guid eventId)
    {
        var model = new EventPublishedViewModel
        {
            ManageEventsLink = Url.RouteUrl(RouteNames.NetworkEvents)!
        };

        _sessionService.Delete(nameof(EventSessionModel));

        var viewPathSuccessfullyPublishedEvent = "~/Views/ManageEvent/EventPublished.cshtml";
        return View(viewPathSuccessfullyPublishedEvent, model);
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

    private Guid GetMemberId()
    {
        var id = Guid.Empty;

        var memberId = _sessionService.Get(SessionKeys.MemberId);

        if (Guid.TryParse(memberId, out var newGuid))
        {
            id = newGuid;
        }

        return id;
    }
}