using SFA.DAS.Admin.Aan.Web.Models.NetworkEvents;

namespace SFA.DAS.Admin.Aan.Web.Services;

public static class QueryStringParameterBuilder
{

    public static Dictionary<string, string[]> BuildQueryStringParameters(GetNetworkEventsRequest request)
    {
        var parameters = new Dictionary<string, string[]>();
        if (request.Page != null) parameters.Add("page", new[] { request.Page?.ToString() }!);
        if (request.PageSize != null) parameters.Add("pageSize", new[] { request.PageSize?.ToString() }!);
        return parameters;
    }
}
