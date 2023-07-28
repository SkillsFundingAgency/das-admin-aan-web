using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Domain.OuterApi.Responses;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvents;

namespace SFA.DAS.Admin.Aan.Web.Controllers;

//[Authorize] //MFCMFC
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
        // var memberId = User.GetAanMemberId();
        // //memberId = new Guid("81C0D92A-F20C-4B0B-B8BF-411B563FB3E5");
        // memberId = new Guid("ac3709c1-aabf-4ea9-b97f-88ccfae4a34e");
        // var calendarEventsTask = _outerApiClient.GetCalendarEvents(memberId, QueryStringParameterBuilder.BuildQueryStringParameters(request), cancellationToken);
        // List<Task> tasks = new() { calendarEventsTask };
        // await Task.WhenAll(tasks);
        //
        // var model = InitialiseViewModel(calendarEventsTask.Result);
        //return View(model);
        return View(new NetworkEventsViewModel());

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
}

