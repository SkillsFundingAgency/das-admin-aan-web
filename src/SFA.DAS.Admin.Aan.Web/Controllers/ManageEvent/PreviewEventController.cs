using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Authentication;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.Controllers.ManageEvent;

[Authorize(Roles = Roles.ManageEventsRole)]
[Route("events/new/preview", Name = RouteNames.ManageEvent.PreviewEvent)]
public class PreviewEventController : Controller
{
    private readonly IOuterApiClient _outerApiClient;
    private readonly ISessionService _sessionService;

    public PreviewEventController(IOuterApiClient outerApiClient, ISessionService sessionService)
    {
        _outerApiClient = outerApiClient;
        _sessionService = sessionService;
    }


    public const string ViewPath = "~/Views/ManageEvent/PreviewEvent.cshtml";

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();

        var model = await GetViewModel(sessionModel, cancellationToken);
        return View(ViewPath, model);
    }

    private async Task<PreviewEventViewModel> GetViewModel(EventSessionModel sessionModel, CancellationToken cancellationToken)
    {
        var calendarTask = _outerApiClient.GetCalendars(cancellationToken);
        var regionTask = _outerApiClient.GetRegions(cancellationToken);

        List<Task> tasks = new() { calendarTask, regionTask };
        await Task.WhenAll(tasks);

        var eventTypes = calendarTask.Result;
        var regions = regionTask.Result.Regions.Select(reg => new RegionSelection(reg.Area, reg.Id)).ToList();
        regions.Add(new RegionSelection("National", 0));

        var model = (PreviewEventViewModel)sessionModel;
        // model.PageTitle = CreateEvent.PageTitle;
        // model.CancelLink = Url.RouteUrl(RouteNames.NetworkEvents)!;
        // model.PostLink = "#";
        // model.PreviewLink = "#";
        model.EventType = eventTypes.First(x => x.Id == sessionModel.CalendarId).CalendarName;
        //   model.EventRegion = regions.First(x => x.RegionId == sessionModel.RegionId).Name;

        // model.EventFormatLink = Url.RouteUrl(RouteNames.ManageEvent.EventFormat)!;
        // model.EventLocationLink = Url.RouteUrl(RouteNames.ManageEvent.Location)!;
        // model.EventTypeLink = Url.RouteUrl(RouteNames.ManageEvent.EventType)!;
        // model.EventDateTimeLink = Url.RouteUrl(RouteNames.ManageEvent.DateAndTime)!;
        // model.EventDescriptionLink = Url.RouteUrl(RouteNames.ManageEvent.Description)!;
        // model.HasGuestSpeakersLink = Url.RouteUrl(RouteNames.ManageEvent.HasGuestSpeakers)!;
        // model.GuestSpeakersListLink = Url.RouteUrl(RouteNames.ManageEvent.GuestSpeakerList)!;
        // model.OrganiserDetailsLink = Url.RouteUrl(RouteNames.ManageEvent.OrganiserDetails)!;
        // model.IsAtSchoolLink = Url.RouteUrl(RouteNames.ManageEvent.IsAtSchool)!;
        // model.SchoolNameLink = Url.RouteUrl(RouteNames.ManageEvent.SchoolName)!;
        // model.NumberOfAttendeesLink = Url.RouteUrl(RouteNames.ManageEvent.NumberOfAttendees)!;
        return model;
    }
}
