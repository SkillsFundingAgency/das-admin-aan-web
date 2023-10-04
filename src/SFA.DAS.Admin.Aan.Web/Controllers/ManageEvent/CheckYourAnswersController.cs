using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Application.Constants;
using SFA.DAS.Admin.Aan.Application.OuterApi.Calendar;
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

        sessionModel.EventFormat = EventFormat.Online;
        sessionModel.EventTitle = "event title 1";
        sessionModel.CalendarId = 3;
        sessionModel.RegionId = 0;
        sessionModel.EventOutline = "event outline";
        sessionModel.EventSummary = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean sit amet bibendum arcu. Aenean tristique ut elit a euismod. Praesent vitae eros vel quam accumsan finibus. Phasellus cursus purus ut elit imperdiet, lacinia rutrum eros sagittis. Nunc commodo velit hendrerit semper ornare. Maecenas ornare neque vitae commodo vehicula. Integer nec congue libero. Phasellus dignissim efficitur massa id vestibulum. Praesent vulputate, enim non varius gravida, nunc purus bibendum nisl, sed iaculis augue augue nec purus. Vestibulum eu diam tincidunt, sagittis risus in, maximus ante.\r\n\r\n*   _first_\r\n*   second\r\n*   **third**\r\n\r\nsecond list\r\n\r\n1.  tenth\r\n2.  eleventh\r\n3.  12";

        sessionModel.HasGuestSpeakers = true;

        sessionModel.GuestSpeakers = new List<GuestSpeaker>
        {
            new GuestSpeaker("Dr Herbert West", "Head of Research and Development for Alternate Life States, Miskatonic University",1 ),
            new GuestSpeaker("Mark C","Senior Dev",2)
        };

        sessionModel.DateOfEvent = DateTime.Today.AddDays(1);
        sessionModel.StartHour = 11;
        sessionModel.StartMinutes = 0;
        sessionModel.EndHour = 13;
        sessionModel.EndMinutes = 30;


        sessionModel.Location = "event location";
        sessionModel.EventLink = "http://www.google.com";
        //sessionModel.EventLink = null;

        sessionModel.IsAtSchool = true;
        sessionModel.SchoolName = "Broadwater school etc etc";

        sessionModel.ContactName = "organiser name";
        sessionModel.ContactEmail = "organiserEmail@test.com";

        sessionModel.PlannedAttendees = 5;

        var model = await GetViewModel(sessionModel, cancellationToken);
        return View(ViewPath, model);
    }
    [HttpPost]
    public async Task<IActionResult> Post(CancellationToken cancellationToken)
    {



        var sessionModel2 = _sessionService.Get<EventSessionModel>();
        // // MFCMFC remove after dev
        var sessionModel = new EventSessionModel();

        sessionModel.EventFormat = EventFormat.Online;
        sessionModel.EventTitle = "event title 1";
        sessionModel.CalendarId = 3;
        sessionModel.RegionId = 0;
        sessionModel.EventOutline = "event outline";
        sessionModel.EventSummary = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean sit amet bibendum arcu. Aenean tristique ut elit a euismod. Praesent vitae eros vel quam accumsan finibus. Phasellus cursus purus ut elit imperdiet, lacinia rutrum eros sagittis. Nunc commodo velit hendrerit semper ornare. Maecenas ornare neque vitae commodo vehicula. Integer nec congue libero. Phasellus dignissim efficitur massa id vestibulum. Praesent vulputate, enim non varius gravida, nunc purus bibendum nisl, sed iaculis augue augue nec purus. Vestibulum eu diam tincidunt, sagittis risus in, maximus ante.\r\n\r\n*   _first_\r\n*   second\r\n*   **third**\r\n\r\nsecond list\r\n\r\n1.  tenth\r\n2.  eleventh\r\n3.  12";

        sessionModel.HasGuestSpeakers = true;

        sessionModel.GuestSpeakers = new List<GuestSpeaker>
        {
            new GuestSpeaker("Dr Herbert West", "Head of Research and Development for Alternate Life States, Miskatonic University",1 ),
            new GuestSpeaker("Mark C","Senior Dev",2)
        };

        sessionModel.DateOfEvent = DateTime.Today.AddDays(1);
        sessionModel.StartHour = 11;
        sessionModel.StartMinutes = 0;
        sessionModel.EndHour = 13;
        sessionModel.EndMinutes = 30;


        sessionModel.Location = "event location";
        sessionModel.EventLink = "http://www.google.com";
        //sessionModel.EventLink = null;

        sessionModel.IsAtSchool = true;
        sessionModel.SchoolName = "Broadwater school etc etc";

        sessionModel.ContactName = "organiser name";
        sessionModel.ContactEmail = "organiserEmail@test.com";

        sessionModel.PlannedAttendees = 5;

        var request = new CreateEventRequest();
        request.EventFormat = sessionModel.EventFormat;
        request.Guests = new List<Guest>();
        foreach (var guest in sessionModel.GuestSpeakers)
        {
            request.Guests.Add(new Guest(guest.GuestName, guest.GuestJobTitle));

        }

        var requestHeader = GetMemberId();

        request.RequestedByMemberId = requestHeader;
        var calendarEventResponse = await _outerApiClient.PostCalendarEvent(requestHeader, request, cancellationToken);

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
        model.EventType = eventTypes.First(x => x.Id == sessionModel.CalendarId).CalendarName;
        model.EventRegion = regions.First(x => x.RegionId == sessionModel.RegionId).Name;

        return model;
    }

    private Guid GetMemberId()
    {
        var id = new Guid();
        var memberId = _sessionService.Get(SessionKeys.MemberId);

        if (Guid.TryParse(memberId, out var newGuid))
        {
            id = newGuid;
        }

        return id;
    }
}