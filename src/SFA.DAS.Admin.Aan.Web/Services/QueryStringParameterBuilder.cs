using SFA.DAS.Admin.Aan.Web.Extensions;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvents;

namespace SFA.DAS.Admin.Aan.Web.Services;

public static class QueryStringParameterBuilder
{
    public static Dictionary<string, string[]> BuildQueryStringParameters(GetNetworkEventsRequest request)
    {
        var parameters = new Dictionary<string, string[]>();
        if (!string.IsNullOrWhiteSpace(request.Location))
        {
            parameters.Add("location", [request.Location]);
            parameters.Add("radius", [request.Radius.ToString() ?? string.Empty]);
        }
        if (request.FromDate != null) parameters.Add("fromDate", [request.FromDate.Value.ToApiString()]);
        if (request.ToDate != null) parameters.Add("toDate", [request.ToDate.Value.ToApiString()]);
        if (request.IsActive.Count == 1)
        {
            parameters.Add("isActive", request.IsActive.Select(isActive => isActive.ToString()).ToArray());
        }
        parameters.Add("calendarId", request.CalendarId.Select(cal => cal.ToString()).ToArray());
        parameters.Add("regionId", request.RegionId.Select(region => region.ToString()).ToArray());
        if (request.Page.HasValue) parameters.Add("page", new[] { request.Page.Value.ToString() }!);
        if (request.PageSize.HasValue) parameters.Add("pageSize", new[] { request.PageSize.Value.ToString() }!);
        if (request.ShowUserEventsOnly.Count == 1)
        {
            parameters.Add("showUserEventsOnly", request.ShowUserEventsOnly.Select(s => s.ToString()).ToArray());
        }
        return parameters;
    }
}
