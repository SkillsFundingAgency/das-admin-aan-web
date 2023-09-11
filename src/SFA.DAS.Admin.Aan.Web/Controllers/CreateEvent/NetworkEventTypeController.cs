using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

namespace SFA.DAS.Admin.Aan.Web.Controllers.CreateEvent;

[Authorize]
[Route("network-event/create-event-type", Name = RouteNames.CreateEvent.EventType)]
public class NetworkEventTypeController : Controller
{
    private readonly IOuterApiClient _outerApiClient;
    private readonly ISessionService _sessionService;
    private readonly IValidator<CreateEventTypeViewModel> _validator;

    public const string ViewPath = "~/Views/NetworkEvent/EventType.cshtml";
    public NetworkEventTypeController(IOuterApiClient outerApiClient, ISessionService sessionService, IValidator<CreateEventTypeViewModel> validator)
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
    public async Task<IActionResult> Post(CreateEventTypeViewModel submitModel, CancellationToken cancellationToken)
    {
        var sessionModel = _sessionService.Get<CreateEventSessionModel?>();
        if (sessionModel == null) return RedirectToAction("Get", "NetworkEventFormat");

        sessionModel.EventTitle = submitModel.EventTitle;
        sessionModel.EventTypeId = submitModel.EventTypeId;
        sessionModel.EventRegionId = submitModel.EventRegionId;

        var result = await _validator.ValidateAsync(submitModel, cancellationToken);

        if (!result.IsValid)
        {

            result.AddToModelState(ModelState);
            return View(ViewPath, await GetViewModel(sessionModel, cancellationToken));
        }

        sessionModel.EventTitle = submitModel.EventTitle;
        _sessionService.Set(sessionModel);
        return RedirectToAction("Get", "NetworkEventType");
    }

    private async Task<CreateEventTypeViewModel> GetViewModel(CreateEventSessionModel sessionModel, CancellationToken cancellationToken)
    {
        var calendarTask = _outerApiClient.GetCalendars(cancellationToken);
        var regionTask = _outerApiClient.GetRegions(cancellationToken);

        List<Task> tasks = new() { calendarTask, regionTask };
        await Task.WhenAll(tasks);

        var eventTypes = calendarTask.Result;
        var regions = regionTask.Result.Regions;

        var eventTypeDropdown = eventTypes.Select(cal => new EventTypeSelection(cal.CalendarName, cal.Id));

        var regionDropdowns = regions.Select(reg => new RegionSelection(reg.Area, reg.Id));

        return new CreateEventTypeViewModel
        {
            EventTitle = sessionModel?.EventTitle,
            EventTypeId = sessionModel?.EventTypeId,
            EventRegionId = sessionModel?.EventRegionId,
            EventTypes = eventTypeDropdown.ToList(),
            EventRegions = regionDropdowns.ToList(),
            BackLink = Url.RouteUrl(RouteNames.NetworkEvents)!
        };
    }
}
