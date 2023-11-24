﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Aan.SharedUi.Models;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Authentication;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.Controllers;

[Authorize(Roles = Roles.ManageEventsRole)]
public class CalendarEventController : Controller
{
    private readonly IOuterApiClient _outerApiClient;
    private readonly ISessionService _sessionService;
    public const string ViewPath = "~/Views/ManageEvent/ReviewEvent.cshtml";
    public const string PreviewViewPath = "~/Views/NetworkEventDetails/Detail.cshtml";

    public CalendarEventController(IOuterApiClient outerApiClient, ISessionService sessionService)
    {
        _outerApiClient = outerApiClient;
        _sessionService = sessionService;
    }

    [HttpGet]
    [Route("/events/{calendarEventId}", Name = RouteNames.CalendarEvent)]
    public async Task<IActionResult> Get(Guid calendarEventId, CancellationToken cancellationToken)
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();

        if (sessionModel == null! || sessionModel.CalendarEventId != calendarEventId)
        {
            var calendarEvent =
                await _outerApiClient.GetCalendarEvent(_sessionService.GetMemberId(), calendarEventId,
                    cancellationToken);

            sessionModel = (EventSessionModel)calendarEvent;
        }

        sessionModel.HasSeenPreview = true;
        sessionModel.IsDirectCallFromCheckYourAnswers = true;
        sessionModel.IsAlreadyPublished = true;

        _sessionService.Set(sessionModel);

        var model = GetViewModel(sessionModel);

        return View(ViewPath, model);
    }


    [HttpPost]
    [Route("events/{calendarEventId}", Name = RouteNames.CalendarEvent)]
    public IActionResult Post()
    {
        return RedirectToRoute(RouteNames.NetworkEvents);
    }

    [HttpGet]
    [Route("events/{calendarEventId}/preview", Name = RouteNames.UpdateEvent.UpdatePreviewEvent)]
    public IActionResult GetPreview()
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();
        var model = GetPreviewModel(sessionModel);
        return View(PreviewViewPath, model);
    }

    private ReviewEventViewModel GetViewModel(EventSessionModel sessionModel)
    {
        var model = (ReviewEventViewModel)sessionModel;
        model.PageTitle = string.Empty;
        model.CancelLink = Url.RouteUrl(RouteNames.NetworkEvents)!;
        model.PostLink = "#";
        model.PreviewLink = Url.RouteUrl(RouteNames.UpdateEvent.UpdatePreviewEvent, new { sessionModel.CalendarEventId })!;

        model.EventType = sessionModel.CalendarName;
        model.EventRegion = sessionModel.RegionName;

        model.EventFormatLink = "#";
        model.EventLocationLink = "#";
        model.EventTypeLink = "#";
        model.EventDateTimeLink = "#";
        model.EventDescriptionLink = "#";
        model.HasGuestSpeakersLink = "#";
        model.GuestSpeakersListLink = "#";
        model.OrganiserDetailsLink = "#";
        model.IsAtSchoolLink = "#";
        model.SchoolNameLink = "#";
        model.NumberOfAttendeesLink = "#";
        return model;
    }

    private NetworkEventDetailsViewModel GetPreviewModel(EventSessionModel sessionModel)
    {
        var model = (NetworkEventDetailsViewModel)sessionModel;

        model.IsPreview = true;
        model.BackLinkUrl = Url.RouteUrl(RouteNames.CalendarEvent, new { sessionModel.CalendarEventId })!;

        return model;
    }

}