﻿using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestEase;
using SFA.DAS.Aan.SharedUi.Models;
using SFA.DAS.Admin.Aan.Application.OuterApi.Calendar.Responses;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Authentication;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;
using SFA.DAS.Admin.Aan.Web.Services;
using SFA.DAS.AdminService.Common.Extensions.TagHelpers;
using static SFA.DAS.Admin.Aan.Web.Models.ManageEvent.ReviewEventViewModel;

namespace SFA.DAS.Admin.Aan.Web.Controllers;

[Authorize(Roles = Roles.ManageEventsRole)]
public class CalendarEventController(
    IOuterApiClient outerApiClient,
    ISessionService sessionService,
    IValidator<ReviewEventViewModel> validator,
    ICsvHelperService csvHelperService)
    : Controller
{
    public const string ViewPath = "~/Views/ManageEvent/ReviewEvent.cshtml";
    public const string PreviewViewPath = "~/Views/NetworkEventDetails/Detail.cshtml";
    public const string EventPreviewHeader = "Event preview";
    public const string PreviewBackLinkDescription = "back to event page";
    public const string EventDetailsHeader = "Event details";
    public const string DetailsBackLinkDescription = "back to manage events";

    [HttpGet]
    [ResponseCache(NoStore = true)]
    [Route("/events/{calendarEventId}", Name = RouteNames.CalendarEvent)]
    public async Task<IActionResult> Get(Guid calendarEventId, CancellationToken cancellationToken, ReviewEventViewModel.AttendeeSortOrderOption sortOrder = ReviewEventViewModel.AttendeeSortOrderOption.SignedUpDescending)
    {
        var sessionModel = sessionService.Get<EventSessionModel>();

        if (sessionModel == null! || sessionModel.CalendarEventId != calendarEventId)
        {
            var calendarEvent =

                await outerApiClient.GetCalendarEvent(sessionService.GetMemberId(), calendarEventId,
                    cancellationToken);
            sessionModel = (EventSessionModel)calendarEvent;
        }

        sessionModel.HasSeenPreview = true;
        sessionModel.IsDirectCallFromCheckYourAnswers = true;
        sessionModel.IsAlreadyPublished = true;

        sessionService.Set(sessionModel);

        var model = await GetViewModel(sessionModel, sortOrder, cancellationToken);

        return View(ViewPath, model);
    }

    [HttpGet]
    [Route("/events/{calendarEventId}/download-attendees", Name = RouteNames.CalendarEventAttendeesDownload)]
    public async Task<IActionResult> GetAttendees([FromRoute] Guid calendarEventId, CancellationToken cancellationToken)
    {
        var attendees =
            await outerApiClient.GetCalendarEventAttendees(sessionService.GetMemberId(), calendarEventId, cancellationToken);

        var data = csvHelperService.GenerateCsvFileFromModel(attendees);

        return new FileContentResult(data, "text/csv");
    }

    [HttpPost]
    [Route("events/{calendarEventId}", Name = RouteNames.CalendarEvent)]
    public async Task<IActionResult> Post(Guid calendarEventId, CancellationToken cancellationToken)
    {
        var sessionModel = sessionService.Get<EventSessionModel>();
        if (sessionModel == null) return RedirectToRoute(RouteNames.NetworkEvents);
        var submitModel = await GetViewModel(sessionModel, ReviewEventViewModel.AttendeeSortOrderOption.SignedUpDescending, cancellationToken);

        var result = await validator.ValidateAsync(submitModel, cancellationToken);

        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            return RedirectToRoute(RouteNames.CalendarEvent, new { calendarEventId});
        }

        return RedirectToRoute(RouteNames.UpdateEvent.NotifyAttendees, new { calendarEventId });
    }

    [HttpGet]
    [Route("events/{calendarEventId}/preview", Name = RouteNames.UpdateEvent.UpdatePreviewEvent)]
    public IActionResult GetPreview()
    {
        var sessionModel = sessionService.Get<EventSessionModel>();
        if (sessionModel == null) return RedirectToRoute(RouteNames.NetworkEvents, null);
        var model = GetPreviewModel(sessionModel);
        return View(PreviewViewPath, model);
    }

    [HttpGet]
    [Route("events/{calendarEventId}/details", Name = RouteNames.EventDetails)]
    public async Task<IActionResult> GetDetails([FromRoute] Guid calendarEventId, CancellationToken cancellationToken)
    {
        var calendarEvent = await outerApiClient.GetCalendarEvent(sessionService.GetMemberId(), calendarEventId, cancellationToken);
        EventSessionModel sessionModel = calendarEvent;
        var model = GetPreviewModel(sessionModel, true);
        return View(PreviewViewPath, model);
    }

    private async Task<ReviewEventViewModel> GetViewModel(EventSessionModel sessionModel, ReviewEventViewModel.AttendeeSortOrderOption sortOrder, CancellationToken cancellationToken)
    {
        var calendarTask = outerApiClient.GetCalendars(cancellationToken);
        var regionTask = outerApiClient.GetRegions(cancellationToken);

        List<Task> tasks = [calendarTask, regionTask];
        await Task.WhenAll(tasks);

        var eventTypes = calendarTask.Result;
        var regions = regionTask.Result.Regions.Select(reg => new RegionSelection(reg.Area, reg.Id)).ToList();
        regions.Add(new RegionSelection("National", 0));

        var model = (ReviewEventViewModel)sessionModel;
        model.PageTitle = string.Empty;
        model.CancelLink = Url.RouteUrl(RouteNames.NetworkEvents)!;
        model.PostLink = "#";
        model.PreviewLink = Url.RouteUrl(RouteNames.UpdateEvent.UpdatePreviewEvent, new { sessionModel.CalendarEventId })!;

        model.EventType = eventTypes.First(x => x.Id == sessionModel.CalendarId).CalendarName;
        sessionModel.RegionId ??= 0;
        model.EventRegion = regions.First(x => x.RegionId == sessionModel.RegionId).Name;

        model.EventFormatLink = Url.RouteUrl(RouteNames.UpdateEvent.UpdateEventFormat, new { sessionModel.CalendarEventId })!;
        model.EventLocationLink = Url.RouteUrl(RouteNames.UpdateEvent.UpdateLocation, new { sessionModel.CalendarEventId })!;
        model.EventTypeLink = Url.RouteUrl(RouteNames.UpdateEvent.UpdateEventType, new { sessionModel.CalendarEventId })!;
        model.EventDateTimeLink = Url.RouteUrl(RouteNames.UpdateEvent.UpdateDateAndTime, new { sessionModel.CalendarEventId })!;
        model.EventDescriptionLink = Url.RouteUrl(RouteNames.UpdateEvent.UpdateDescription, new { sessionModel.CalendarEventId })!;
        model.HasGuestSpeakersLink = Url.RouteUrl(RouteNames.UpdateEvent.UpdateHasGuestSpeakers, new { sessionModel.CalendarEventId })!;
        model.GuestSpeakersListLink = Url.RouteUrl(RouteNames.UpdateEvent.UpdateGuestSpeakerList, new { sessionModel.CalendarEventId })!;
        model.OrganiserDetailsLink = Url.RouteUrl(RouteNames.UpdateEvent.UpdateOrganiserDetails, new { sessionModel.CalendarEventId })!;
        model.IsAtSchoolLink = Url.RouteUrl(RouteNames.UpdateEvent.UpdateIsAtSchool, new { sessionModel.CalendarEventId })!;
        model.SchoolNameLink = Url.RouteUrl(RouteNames.UpdateEvent.UpdateSchoolName, new { sessionModel.CalendarEventId })!;
        model.NumberOfAttendeesLink = Url.RouteUrl(RouteNames.UpdateEvent.UpdateNumberOfAttendees, new { sessionModel.CalendarEventId })!;
        model.DownloadAttendeesLink = Url.RouteUrl(RouteNames.CalendarEventAttendeesDownload, new { sessionModel.CalendarEventId })!;

        model.SortOrder = sortOrder;
        model.Attendees = Sort(model.Attendees, sortOrder).ToList();

        return model;
    }

    private IOrderedEnumerable<Attendee> Sort(List<Attendee> source, AttendeeSortOrderOption sortOrder)
    {
        switch (sortOrder)
        {
            case AttendeeSortOrderOption.SignedUpDescending:
                return source.OrderByDescending(x => x.SignUpDate);
            case AttendeeSortOrderOption.SignedUpAscending:
                return source.OrderBy(x => x.SignUpDate);
            case AttendeeSortOrderOption.SurnameAsc:
                return source.OrderBy(x => x.Surname);
            case AttendeeSortOrderOption.SurnameDesc:
                return source.OrderByDescending(x => x.Surname);
            default:
                return source.OrderByDescending(x => x.SignUpDate);
        }
    }

    private NetworkEventDetailsViewModel GetPreviewModel(EventSessionModel sessionModel, bool IsShowingDetails = false)
    {
        var model = (NetworkEventDetailsViewModel)sessionModel;
        model.IsPreview = true;
        if (IsShowingDetails)
        {
            model.BackLinkUrl = Url.RouteUrl(RouteNames.NetworkEvents)!;
            model.PreviewHeader = EventDetailsHeader;
            model.BackLinkDescription = DetailsBackLinkDescription;
        }
        else
        {
            model.BackLinkUrl = Url.RouteUrl(RouteNames.CalendarEvent, new { sessionModel.CalendarEventId })!;
            model.PreviewHeader = EventPreviewHeader;
            model.BackLinkDescription = PreviewBackLinkDescription;
        }

        return model;
    }
}
