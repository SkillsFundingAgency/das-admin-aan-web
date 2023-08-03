using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Web.Extensions;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvents;
using System.Globalization;

namespace SFA.DAS.Admin.Aan.Web.Services;

public static class FilterBuilder
{
    public static List<SelectedFilter> Build(GetNetworkEventsRequest request, IUrlHelper urlHelper, IEnumerable<ChecklistLookup> eventStatusLookups)
    {
        var filters = new List<SelectedFilter>();
        var fullQueryParameters = BuildQueryParameters(request);

        if (request.FromDate.HasValue)
        {
            filters.AddFilterItems(urlHelper, fullQueryParameters, new[] { request.FromDate.Value.ToScreenString() }, "From date", "fromDate", Enumerable.Empty<ChecklistLookup>());
        }

        if (request.ToDate.HasValue)
        {
            filters.AddFilterItems(urlHelper, fullQueryParameters, new[] { request.ToDate.Value.ToScreenString() }, "To date", "toDate", Enumerable.Empty<ChecklistLookup>());
        }

        filters.AddFilterItems(urlHelper, fullQueryParameters, request.IsActive.Select(e => e.ToString()), "Event status", "isActive", eventStatusLookups);


        return filters;
    }

    public static string BuildFullQueryString(GetNetworkEventsRequest request, IUrlHelper url)
    {
        var fullQueryParameters = BuildQueryParameters(request);
        return BuildQueryString(url, fullQueryParameters, "none")!;
    }

    private static void AddFilterItems(this ICollection<SelectedFilter> filters, IUrlHelper url, List<string> fullQueryParameters, IEnumerable<string> selectedValues, string fieldName, string parameterName, IEnumerable<ChecklistLookup> lookups)
    {
        if (!selectedValues.Any()) return;

        var filter = new SelectedFilter
        {
            FieldName = fieldName,
            FieldOrder = filters.Count + 1
        };

        int i = 0;

        foreach (var value in selectedValues)
        {
            var v = value;
            filter.Filters.Add(BuildFilterItem(url, fullQueryParameters, BuildQueryParameter(parameterName, value), v, ++i));
        }

        filters.Add(filter);
    }

    private static List<string> BuildQueryParameters(GetNetworkEventsRequest request)
    {
        var queryParameters = new List<string>();

        if (request.FromDate != null)
        {
            queryParameters.Add(BuildDateQueryParameter("fromDate", request.FromDate.GetValueOrDefault()));
        }

        if (request.ToDate != null)
        {
            queryParameters.Add(BuildDateQueryParameter("toDate", request.ToDate.GetValueOrDefault()));
        }

        queryParameters.AddRange(request.IsActive.Select(isActive => "isActive=" + isActive));

        return queryParameters;
    }

    private static EventFilterItem BuildFilterItem(IUrlHelper url, List<string> queryParameters, string filterToRemove, string filterValue, int filterFieldOrder)
        =>
            new()
            {
                ClearFilterLink = BuildQueryString(url, queryParameters, filterToRemove),
                Order = filterFieldOrder,
                Value = filterValue
            };

    private static string BuildDateQueryParameter(string name, DateTime value) => $"{name}={value.ToApiString()}";

    private static string BuildQueryParameter(string name, string value)
    {
        return DateTime.TryParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,
            out DateTime parsedDate) ?
            $"{name}={parsedDate.ToApiString()}" :
            $"{name}={value}";
    }

    private static string? BuildQueryString(IUrlHelper url, List<string> queryParameters, string filterToRemove)
    {
        var queryParametersToBuild = queryParameters.Where(s => s != filterToRemove);

        return queryParametersToBuild.Any() ? $"{url.RouteUrl(RouteNames.NetworkEvents)}?{string.Join('&', queryParametersToBuild)}" : url.RouteUrl(RouteNames.NetworkEvents);
    }
}
