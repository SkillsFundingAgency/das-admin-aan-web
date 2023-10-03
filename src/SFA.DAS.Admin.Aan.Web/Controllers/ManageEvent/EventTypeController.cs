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
[Route("events/new/type", Name = RouteNames.ManageEvent.EventType)]
public class EventTypeController : Controller
{
    private readonly IOuterApiClient _outerApiClient;
    private readonly ISessionService _sessionService;
    private readonly IValidator<ManageEventTypeViewModel> _validator;

    public const string ViewPath = "~/Views/ManageEvent/EventType.cshtml";
    public EventTypeController(IOuterApiClient outerApiClient, ISessionService sessionService, IValidator<ManageEventTypeViewModel> validator)
    {
        _outerApiClient = outerApiClient;
        _sessionService = sessionService;
        _validator = validator;
    }
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();
        var model = await GetViewModel(sessionModel, cancellationToken);
        return View(ViewPath, model);
    }

    [HttpPost]
    public async Task<IActionResult> Post(ManageEventTypeViewModel submitModel, CancellationToken cancellationToken)
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
        _sessionService.Set(sessionModel);
        return RedirectToRoute(RouteNames.ManageEvent.Description);
    }

    private async Task<ManageEventTypeViewModel> GetViewModel(EventSessionModel sessionModel, CancellationToken cancellationToken)
    {
        var calendarTask = _outerApiClient.GetCalendars(cancellationToken);
        var regionTask = _outerApiClient.GetRegions(cancellationToken);

        List<Task> tasks = new() { calendarTask, regionTask };
        await Task.WhenAll(tasks);

        var eventTypes = calendarTask.Result.OrderBy(x => x.CalendarName);
        var regions = regionTask.Result.Regions;

        var eventTypeDropdown = eventTypes.Select(cal => new EventTypeSelection(cal.CalendarName, cal.Id));

        var regionDropdowns = regions.Select(reg => new RegionSelection(reg.Area, reg.Id));

        var regionsWithNational = regionDropdowns.ToList();
        regionsWithNational.Add(new RegionSelection("National", 0));
        return new ManageEventTypeViewModel
        {
            EventTitle = sessionModel.EventTitle,
            EventTypeId = sessionModel.CalendarId,
            EventRegionId = sessionModel.RegionId,
            EventTypes = eventTypeDropdown.ToList(),
            EventRegions = regionsWithNational,
            CancelLink = Url.RouteUrl(RouteNames.NetworkEvents)!,
            PostLink = Url.RouteUrl(RouteNames.ManageEvent.EventType)!,
            PageTitle = Application.Constants.CreateEvent.PageTitle
        };
    }
}
