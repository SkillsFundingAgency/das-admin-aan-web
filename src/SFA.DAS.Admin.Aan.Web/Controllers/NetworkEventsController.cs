using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Application.OuterApi.Regions;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Domain.OuterApi.Responses;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvents;
using SFA.DAS.Admin.Aan.Web.Services;
using Region = SFA.DAS.Admin.Aan.Application.OuterApi.Regions.Region;

namespace SFA.DAS.Admin.Aan.Web.Controllers;

[Authorize]
[Route("network-events")]
public class NetworkEventsController : Controller
{
    private readonly IOuterApiClient _outerApiClient;

    public NetworkEventsController(IOuterApiClient outerApiClient)
    {
        _outerApiClient = outerApiClient;
    }

    [HttpGet]
    [Route("", Name = RouteNames.NetworkEvents)]
    public async Task<IActionResult> Index(GetNetworkEventsRequest request, CancellationToken cancellationToken)
    {
        //var memberId = User.GetAanMemberId();
        //memberId = new Guid("81C0D92A-F20C-4B0B-B8BF-411B563FB3E5");
        var memberId = new Guid("ac3709c1-aabf-4ea9-b97f-88ccfae4a34e");
        var filterUrl = FilterBuilder.BuildFullQueryString(request, Url);
        var calendarEventsTask = _outerApiClient.GetCalendarEvents(memberId, QueryStringParameterBuilder.BuildQueryStringParameters(request), cancellationToken);
        var calendarTask = _outerApiClient.GetCalendars(cancellationToken);
        var regionTask = _outerApiClient.GetRegions(cancellationToken);

        List<Task> tasks = new() { calendarEventsTask, calendarTask, regionTask };

        await Task.WhenAll(tasks);

        var calendars = calendarTask.Result;
        var regions = regionTask.Result.Regions;

        var model = InitialiseViewModel(calendarEventsTask.Result);

        model.PaginationViewModel = SetupPagination(calendarEventsTask.Result, filterUrl!);
        var filterChoices = PopulateFilterChoices(request, calendars, regions);
        model.FilterChoices = filterChoices;
        model.SelectedFilters = FilterBuilder.Build(request, Url, filterChoices.EventStatusChecklistDetails.Lookups, filterChoices.EventTypeChecklistDetails.Lookups, filterChoices.RegionChecklistDetails.Lookups);

        return View(model);
    }

    private static NetworkEventsViewModel InitialiseViewModel(GetCalendarEventsQueryResult result)
    {
        var model = new NetworkEventsViewModel
        {
            TotalCount = result.TotalCount
        };

        foreach (var calendarEvent in result.CalendarEvents)
        {
            CalendarEventViewModel vm = calendarEvent;
            model.CalendarEvents.Add(vm);
        }
        return model;
    }

    private static PaginationViewModel SetupPagination(GetCalendarEventsQueryResult result, string filterUrl)
    {
        return new PaginationViewModel(result.Page, result.PageSize, result.TotalPages, filterUrl);
    }

    private static EventFilterChoices PopulateFilterChoices(GetNetworkEventsRequest request, IEnumerable<Calendar> calendars, IEnumerable<Region> regions)
        => new()
        {
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            EventStatusChecklistDetails = new ChecklistDetails
            {
                Title = "Event status",
                QueryStringParameterName = "isActive",
                Lookups = new ChecklistLookup[]
                {
                    new("Published", true.ToString(), request.IsActive.Exists(x => x == true)),
                    new("Cancelled", false.ToString(), request.IsActive.Exists(x => x == false)),
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
            }
        };
}

