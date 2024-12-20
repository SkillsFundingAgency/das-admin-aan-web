﻿using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Aan.SharedUi.Models;
using SFA.DAS.Admin.Aan.Application.Constants;
using SFA.DAS.Admin.Aan.Application.OuterApi.Calendar;
using SFA.DAS.Admin.Aan.Application.OuterApi.Calendar.Responses;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Authentication;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.DeleteEvent;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvents;
using SFA.DAS.Admin.Aan.Web.Services;
using Region = SFA.DAS.Admin.Aan.Application.OuterApi.Regions.Region;

namespace SFA.DAS.Admin.Aan.Web.Controllers;

[Authorize(Roles = Roles.ManageEventsRole)]
[Route("events", Name = RouteNames.NetworkEvents)]
public class NetworkEventsController(
    IOuterApiClient outerApiClient,
    ISessionService sessionService,
    IValidator<CancelEventViewModel> validator)
    : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(GetNetworkEventsRequest request, CancellationToken cancellationToken)
    {
        sessionService.Delete(nameof(EventSessionModel));

        var filterUrl = FilterBuilder.BuildFullQueryString(request, Url);
        var calendarEvents = await outerApiClient.GetCalendarEvents(sessionService.GetMemberId(), QueryStringParameterBuilder.BuildQueryStringParameters(request), cancellationToken);

        var calendars = calendarEvents.Calendars;
        var regions = calendarEvents.Regions;

        regions.Add(new Region(0, Application.Constants.Region.National, 100));
        var model = InitialiseViewModel(calendarEvents);

        model.PaginationViewModel = SetupPagination(calendarEvents, filterUrl!);
        var filterChoices = PopulateFilterChoices(request, calendars, regions);
        model.FilterChoices = filterChoices;
        model.OrderBy = request.OrderBy;
        model.IsInvalidLocation = calendarEvents.IsInvalidLocation;
        model.SearchedLocation = (request.Location != null) ? request.Location : string.Empty;
        model.SelectedFilters = FilterBuilder.Build(request, Url, filterChoices.EventStatusChecklistDetails.Lookups, filterChoices.EventTypeChecklistDetails.Lookups, filterChoices.RegionChecklistDetails.Lookups, filterChoices.ShowUserEventsOnlyChecklistDetails.Lookups);
        model.ClearSelectedFiltersLink = Url.RouteUrl(RouteNames.NetworkEvents)!;

        return View(model);
    }

    [HttpGet]
    [Route("create-event", Name = RouteNames.CreateEvent.CreateNewEvent)]
    public IActionResult CreateEvent()
    {
        var sessionModel = new EventSessionModel();
        sessionService.Set(sessionModel);
        return RedirectToRoute(RouteNames.CreateEvent.EventFormat);
    }

    [HttpGet]
    [Route("{calendarEventId}/cancel", Name = RouteNames.DeleteEvent)]
    public async Task<IActionResult> CancelEvent(Guid calendarEventId, CancellationToken cancellationToken)
    {
        var calendarEvent =
            await outerApiClient.GetCalendarEvent(sessionService.GetMemberId(), calendarEventId, cancellationToken);

        var model = new CancelEventViewModel
        {
            CalendarEventId = calendarEventId,
            Title = calendarEvent.Title,
            PostLink = Url.RouteUrl(RouteNames.DeleteEvent)!,
            ManageEventsLink = Url.RouteUrl(RouteNames.NetworkEvents)!
        };

        return View(model);
    }

    [HttpPost]
    [Route("{calendarEventId}/cancel", Name = RouteNames.DeleteEvent)]
    public async Task<IActionResult> PostCancelEvent(CancelEventViewModel submitModel, CancellationToken cancellationToken)
    {
        const string viewPath = "~/Views/NetworkEvents/CancelEvent.cshtml";
        var result = validator.Validate(submitModel);

        if (!result.IsValid)
        {

            result.AddToModelState(ModelState);
            return View(viewPath, submitModel);
        }

        await outerApiClient.DeleteCalendarEvent(sessionService.GetMemberId(), submitModel.CalendarEventId, cancellationToken);

        return RedirectToRoute(RouteNames.DeleteEventConfirmation, new { calendarEventId = submitModel.CalendarEventId });
    }

    [HttpGet]
    [Route("{calendarEventId}/cancel-confirmed", Name = RouteNames.DeleteEventConfirmation)]
    public IActionResult DeleteEventConfirmation()
    {
        const string viewPath = "~/Views/NetworkEvents/CancelEventConfirmation.cshtml";
        var model = new CancelEventConfirmationViewModel
        {
            ManageEventsLink = Url.RouteUrl(RouteNames.NetworkEvents)
        };

        return View(viewPath, model);
    }

    private NetworkEventsViewModel InitialiseViewModel(GetCalendarEventsQueryResult result)
    {
        var model = new NetworkEventsViewModel
        {
            TotalCount = result.TotalCount,
            CreateEventLink = Url.RouteUrl(RouteNames.CreateEvent.EventFormat)!
        };

        foreach (var calendarEvent in result.CalendarEvents)
        {
            CalendarEventViewModel vm = calendarEvent;
            vm.CancelEventLink = Url.RouteUrl(RouteNames.DeleteEvent, new { calendarEvent.CalendarEventId });
            vm.EditEventLink = Url.RouteUrl(RouteNames.CalendarEvent, new { calendarEvent.CalendarEventId });
            vm.ViewDetailsLink = Url.RouteUrl(RouteNames.EventDetails, new { calendarEvent.CalendarEventId });
            model.CalendarEvents.Add(vm);
        }
        return model;
    }

    private static PaginationViewModel SetupPagination(GetCalendarEventsQueryResult result, string filterUrl)
    {
        return new PaginationViewModel(result.Page, result.PageSize, result.TotalPages, filterUrl);
    }

    private static EventFilterChoices PopulateFilterChoices(GetNetworkEventsRequest request, IEnumerable<CalendarDetail> calendars, IEnumerable<Region> regions)
        => new()
        {
            Location = request.Location ?? "",
            Radius = request.Radius ?? 5,
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            EventStatusChecklistDetails = new ChecklistDetails
            {
                Title = "Event status",
                QueryStringParameterName = "isActive",
                Lookups = new ChecklistLookup[]
                {
                    new(EventStatus.Published, true.ToString(), request.IsActive.Exists(x => x)),
                    new(EventStatus.Cancelled, false.ToString(), request.IsActive.Exists(x => !x))
                }
            },
            EventTypeChecklistDetails = new ChecklistDetails
            {
                Title = "Event type",
                QueryStringParameterName = "calendarId",
                Lookups = calendars.OrderBy(x => x.Ordering).Select(cal => new ChecklistLookup(cal.CalendarName, cal.Id.ToString(), request.CalendarId.Exists(x => x == cal.Id))).ToList(),
            },
            RegionChecklistDetails = new ChecklistDetails
            {
                Title = "Regions",
                QueryStringParameterName = "regionId",
                Lookups = regions.OrderBy(x => x.Ordering).Select(region => new ChecklistLookup(region.Area, region.Id.ToString(), request.RegionId.Exists(x => x == region.Id))).ToList()
            },
            ShowUserEventsOnlyChecklistDetails = new ChecklistDetails
            {
                Title = "",
                QueryStringParameterName = "showUserEventsOnly",
                Lookups = new List<ChecklistLookup>
                {
                    new("Only show my events",true.ToString(),  request.ShowUserEventsOnly.FirstOrDefault())
                }
            }
        };
}

