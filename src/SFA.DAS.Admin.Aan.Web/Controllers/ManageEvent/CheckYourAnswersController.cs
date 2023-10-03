using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Application.Constants;
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

    [Authorize(Roles = Roles.ManageEventsRole)]
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var sessionModel2 = _sessionService.Get<EventSessionModel>();


        // MFCMFC remove after dev
        var sessionModel = new EventSessionModel();

        sessionModel.EventFormat = EventFormat.InPerson;
        sessionModel.EventTitle = "event title 1";
        sessionModel.EventTypeId = 3;
        sessionModel.EventRegionId = 9;
        sessionModel.EventOutline = "event outline";
        sessionModel.EventSummary = "event summary";

        sessionModel.HasGuestSpeakers = true;

        sessionModel.GuestSpeakers = new List<GuestSpeaker>
        {
            new GuestSpeaker("Joe Cool", "feeelin cool",1 ),
            new GuestSpeaker("Mark C","Senior Dev",2)
        };

        sessionModel.DateOfEvent = DateTime.Today.AddDays(1);
        sessionModel.StartHour = 11;
        sessionModel.StartMinutes = 0;
        sessionModel.EndHour = 13;
        sessionModel.EndMinutes = 30;


        sessionModel.EventLocation = "event location";
        sessionModel.OnlineEventLink = "http://www.google.com";

        sessionModel.IsAtSchool = true;
        sessionModel.SchoolName = "Broadwater school etc etc";

        sessionModel.OrganiserName = "organiser name";
        sessionModel.OrganiserEmail = "organiserEmail@test.com";

        sessionModel.NumberOfAttendees = 5;


        var model = await GetViewModel(sessionModel, cancellationToken);
        return View(ViewPath, model);
    }

    [HttpPost]
    public IActionResult Post(CheckYourAnswersViewModel submitModel)
    {

        return RedirectToRoute(RouteNames.ManageEvent.CheckYourAnswers);
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
        model.EventType = eventTypes.First(x => x.Id == sessionModel.EventTypeId).CalendarName;
        model.EventRegion = regions.First(x => x.RegionId == sessionModel.EventRegionId).Name;

        return model;
    }
}