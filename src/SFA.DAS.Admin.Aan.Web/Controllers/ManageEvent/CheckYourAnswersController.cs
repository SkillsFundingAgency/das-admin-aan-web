using FluentValidation;
using FluentValidation.AspNetCore;
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
[Route("events/new/check-your-answers", Name = RouteNames.CreateEvent.CheckYourAnswers)]
public class CheckYourAnswersController : Controller
{
    private readonly IOuterApiClient _outerApiClient;
    private readonly ISessionService _sessionService;
    private readonly IValidator<ReviewEventViewModel> _validator;


    public const string ViewPath = "~/Views/ManageEvent/CheckYourAnswers.cshtml";

    public CheckYourAnswersController(ISessionService sessionService, IOuterApiClient outerApiClient, IValidator<ReviewEventViewModel> validator)
    {
        _sessionService = sessionService;
        _outerApiClient = outerApiClient;
        _validator = validator;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();

        if (!sessionModel.HasSeenPreview || !sessionModel.IsDirectCallFromCheckYourAnswers)
        {
            sessionModel.HasSeenPreview = true;
            sessionModel.IsDirectCallFromCheckYourAnswers = true;
            _sessionService.Set(sessionModel);
        }

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

        return RedirectToRoute(RouteNames.CreateEvent.EventPublished, new { eventId = calendarEventResponse.CalendarEventId });
    }

    private async Task<CheckAnswersViewModel> GetViewModel(EventSessionModel sessionModel, CancellationToken cancellationToken)
    {
        var calendarTask = _outerApiClient.GetCalendars(cancellationToken);
        var regionTask = _outerApiClient.GetRegions(cancellationToken);

        List<Task> tasks = [calendarTask, regionTask];
        await Task.WhenAll(tasks);

        var eventTypes = calendarTask.Result;
        var regions = regionTask.Result.Regions.Select(reg => new RegionSelection(reg.Area, reg.Id)).ToList();
        regions.Add(new RegionSelection("National", 0));

        var model = (CheckAnswersViewModel)sessionModel;
        model.PageTitle = CreateEvent.PageTitle;
        model.CancelLink = Url.RouteUrl(RouteNames.NetworkEvents)!;
        model.PostLink = "#";
        model.PreviewLink = Url.RouteUrl(RouteNames.CreateEvent.PreviewEvent)!;
        model.EventType = eventTypes.First(x => x.Id == sessionModel.CalendarId).CalendarName;
        model.EventRegion = regions.First(x => x.RegionId == sessionModel.RegionId).Name;

        model.EventFormatLink = Url.RouteUrl(RouteNames.CreateEvent.EventFormat)!;
        model.EventLocationLink = Url.RouteUrl(RouteNames.CreateEvent.Location)!;
        model.EventTypeLink = Url.RouteUrl(RouteNames.CreateEvent.EventType)!;
        model.EventDateTimeLink = Url.RouteUrl(RouteNames.CreateEvent.DateAndTime)!;
        model.EventDescriptionLink = Url.RouteUrl(RouteNames.CreateEvent.Description)!;
        model.HasGuestSpeakersLink = Url.RouteUrl(RouteNames.CreateEvent.HasGuestSpeakers)!;
        model.GuestSpeakersListLink = Url.RouteUrl(RouteNames.CreateEvent.GuestSpeakerList)!;
        model.OrganiserDetailsLink = Url.RouteUrl(RouteNames.CreateEvent.OrganiserDetails)!;
        model.IsAtSchoolLink = Url.RouteUrl(RouteNames.CreateEvent.IsAtSchool)!;
        model.SchoolNameLink = Url.RouteUrl(RouteNames.CreateEvent.SchoolName)!;
        model.NumberOfAttendeesLink = Url.RouteUrl(RouteNames.CreateEvent.NumberOfAttendees)!;
        return model;
    }
}