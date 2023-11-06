using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Aan.SharedUi.Constants;
using SFA.DAS.Aan.SharedUi.Models;
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


    //public const string ViewPath = "~/Views/PreviewEvent/PreviewEvent.cshtml";
    public const string DetailsViewPath = "~/Views/NetworkEventDetails/Detail.cshtml";


    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        // var sessionModel = _sessionService.Get<EventSessionModel>();
        var sessionModel = new EventSessionModel();
        sessionModel.CalendarId = 6;
        sessionModel.ContactEmail = "rtyrtyrt@test.com";
        sessionModel.ContactName = "Joey";
        sessionModel.DateOfEvent = new DateTime(2023, 11, 25, 0, 0, 0);
        //sessionModel.End -- calculated
        sessionModel.EndHour = 13;
        sessionModel.EndMinutes = 30;
        sessionModel.EventFormat = EventFormat.InPerson;
        sessionModel.EventLink = "https://www.google.com";
        sessionModel.EventOutline = "ghgfg outline";
        sessionModel.EventSummary = "event summary";
        sessionModel.EventTitle = "event title";
        sessionModel.GuestSpeakers = new List<GuestSpeaker>
        {
            new GuestSpeaker("joe cool", "feelin cool", 1),
            new GuestSpeaker("Emma Smith", "Head of AI", 2)
        };
        sessionModel.HasGuestSpeakers = true;
        sessionModel.HasSeenPreview = true; // not needed
        sessionModel.IsAtSchool = true;
        sessionModel.IsDirectCallFromCheckYourAnswers = true; // not needed
        sessionModel.Latitude = 51.8138119;
        sessionModel.Location = "Delfryn, Caerfyrddin";
        sessionModel.Longitude = 4.279403;
        sessionModel.PlannedAttendees = 3;
        sessionModel.Postcode = "SA32 8EB";
        sessionModel.RegionId = 1; // deal with 0
        sessionModel.SchoolName = "Ryde Children's Centre";
        // sessionModel.Start -- calculated
        sessionModel.StartHour = 12;
        sessionModel.StartMinutes = 0;
        sessionModel.Urn = "20004";

        var model = await GetViewModel(sessionModel, cancellationToken);
        return View(DetailsViewPath, model);
    }

    private async Task<NetworkEventDetailsViewModel> GetViewModel(EventSessionModel sessionModel, CancellationToken cancellationToken)
    {
        var calendars = await _outerApiClient.GetCalendars(cancellationToken);

        var eventTypes = calendars;

        sessionModel.CalendarName = eventTypes.First(x => x.Id == sessionModel.CalendarId).CalendarName;
        var model = (NetworkEventDetailsViewModel)sessionModel;

        model.CheckYourAnswersUrl = Url.RouteUrl(RouteNames.ManageEvent.CheckYourAnswers)!;

        return model;
    }
}
