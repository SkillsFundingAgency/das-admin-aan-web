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
public class EventTypeController : Controller
{
    private readonly IOuterApiClient _outerApiClient;
    private readonly ISessionService _sessionService;
    private readonly IValidator<EventTypeViewModel> _validator;

    public const string ViewPath = "~/Views/ManageEvent/EventType.cshtml";
    public EventTypeController(IOuterApiClient outerApiClient, ISessionService sessionService, IValidator<EventTypeViewModel> validator)
    {
        _outerApiClient = outerApiClient;
        _sessionService = sessionService;
        _validator = validator;
    }
    [HttpGet]
    [Route("events/new/type", Name = RouteNames.CreateEvent.EventType)]
    [Route("events/{calendarEventId}/type", Name = RouteNames.UpdateEvent.UpdateEventType)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();
        var model = await GetViewModel(sessionModel, cancellationToken);
        return View(ViewPath, model);
    }

    [HttpPost]
    [Route("events/new/type", Name = RouteNames.CreateEvent.EventType)]
    [Route("events/{calendarEventId}/type", Name = RouteNames.UpdateEvent.UpdateEventType)]
    public async Task<IActionResult> Post(EventTypeViewModel submitModel, CancellationToken cancellationToken)
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();
        sessionModel.EventTitle = submitModel.EventTitle;
        sessionModel.CalendarId = submitModel.EventTypeId;
        sessionModel.RegionId = submitModel.EventRegionId;

        var result = await _validator.ValidateAsync(submitModel, cancellationToken);

        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            return View(ViewPath, await GetViewModel(sessionModel, cancellationToken));
        }

        sessionModel.EventTitle = submitModel.EventTitle;

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

        return RedirectToRoute(RouteNames.CreateEvent.Description);
    }

    private async Task<EventTypeViewModel> GetViewModel(EventSessionModel sessionModel, CancellationToken cancellationToken)
    {
        var calendarTask = _outerApiClient.GetCalendars(cancellationToken);
        var regionTask = _outerApiClient.GetRegions(cancellationToken);

        List<Task> tasks = [calendarTask, regionTask];
        await Task.WhenAll(tasks);

        var eventTypes = calendarTask.Result.OrderBy(x => x.CalendarName);
        var regions = regionTask.Result.Regions;

        var eventTypeDropdown = eventTypes.Select(cal => new EventTypeSelection(cal.CalendarName, cal.Id));

        var regionDropdowns = regions.Select(reg => new RegionSelection(reg.Area, reg.Id));

        var regionsWithNational = regionDropdowns.ToList();
        regionsWithNational.Add(new RegionSelection("National", 0));

        var cancelLink = Url.RouteUrl(RouteNames.NetworkEvents)!;
        var postLink = Url.RouteUrl(RouteNames.CreateEvent.EventType)!;

        if (sessionModel.IsAlreadyPublished)
        {
            cancelLink = Url.RouteUrl(RouteNames.CalendarEvent, new { sessionModel.CalendarEventId });
            postLink = Url.RouteUrl(RouteNames.UpdateEvent.UpdateEventFormat,
                new { sessionModel.CalendarEventId });
        }
        else
        {
            if (sessionModel.HasSeenPreview)
            {
                cancelLink = Url.RouteUrl(RouteNames.CreateEvent.CheckYourAnswers)!;
            }
        }

        return new EventTypeViewModel
        {
            EventTitle = sessionModel.EventTitle?.Trim(),
            EventTypeId = sessionModel.CalendarId,
            EventRegionId = sessionModel.RegionId,
            EventTypes = eventTypeDropdown.ToList(),
            EventRegions = regionsWithNational,
            CancelLink = cancelLink,
            PostLink = postLink,
            PageTitle = sessionModel.PageTitle,
        };
    }
}