using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Web.Extensions;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvents;

namespace SFA.DAS.Admin.Aan.Web.Services;

public static class FilterBuilder
{
    public static List<SelectedFilter> Build(GetNetworkEventsRequest request, IUrlHelper urlHelper, IEnumerable<ChecklistLookup> eventStatusLookups, IEnumerable<ChecklistLookup> eventTypeLookups, IEnumerable<ChecklistLookup> regionLookups, IEnumerable<ChecklistLookup> showUserEventsOnlyLookups)
    {
        var filters = new List<SelectedFilter>();
        var fullQueryParameters = BuildQueryParameters(request);

        if (!string.IsNullOrWhiteSpace(request.Location))
        {
            var text = request.Radius == -1 ? "Across England" : $"Within {request.Radius} miles of {request.Location}";
            filters.AddFilterItems(urlHelper, fullQueryParameters, new [] { text }, "Location", "location", Enumerable.Empty<ChecklistLookup>());
        }

        if (request.FromDate.HasValue)
        {
            filters.AddFilterItems(urlHelper, fullQueryParameters, new[] { request.FromDate.Value.ToScreenString() }, "From date", "fromDate", Enumerable.Empty<ChecklistLookup>());
        }

        if (request.ToDate.HasValue)
        {
            filters.AddFilterItems(urlHelper, fullQueryParameters, new[] { request.ToDate.Value.ToScreenString() }, "To date", "toDate", Enumerable.Empty<ChecklistLookup>());
        }

        filters.AddFilterItems(urlHelper, fullQueryParameters, request.IsActive.Select(e => e.ToString()), "Event status", "isActive", eventStatusLookups);
        filters.AddFilterItems(urlHelper, fullQueryParameters, request.CalendarId.Select(e => e.ToString()), "Event type", "calendarId", eventTypeLookups);
        filters.AddFilterItems(urlHelper, fullQueryParameters, request.RegionId.Select(e => e.ToString()), "Region", "regionId", regionLookups);
        filters.AddFilterItems(urlHelper, fullQueryParameters, request.ShowUserEventsOnly.Select(e => e.ToString()), "", "showUserEventsOnly", showUserEventsOnlyLookups);
        return filters;
    }

    public static string BuildFullQueryString(GetNetworkEventsRequest request, IUrlHelper url)
    {
        var fullQueryParameters = BuildQueryParameters(request);
        return BuildQueryString(url, fullQueryParameters, "none")!;
    }

    private static void AddFilterItems(this List<SelectedFilter> filters, IUrlHelper url, List<string> fullQueryParameters, IEnumerable<string> selectedValues, string fieldName, string parameterName, IEnumerable<ChecklistLookup> lookups)
    {
        if (!selectedValues.Any()) return;

        var filter = new SelectedFilter
        {
            FieldName = fieldName,
            FieldOrder = filters.Count + 1
        };

        var i = 0;

        foreach (var value in selectedValues)
        {
            var v = lookups.Any() ? lookups.First(l => l.Value == value).Name : value;
            filter.Filters.Add(BuildFilterItem(url, fullQueryParameters, BuildQueryParameter(parameterName, value), v, ++i));
        }

        filters.Add(filter);
    }

    private static List<string> BuildQueryParameters(GetNetworkEventsRequest request)
    {
        var queryParameters = new List<string>();

        if (!string.IsNullOrWhiteSpace(request.Location))
        {
            queryParameters.Add($"location={request.Location}");
            queryParameters.Add($"radius={request.Radius}");
        }

        if (request.FromDate != null)
        {
            queryParameters.Add(BuildDateQueryParameter("fromDate", request.FromDate.GetValueOrDefault()));
        }

        if (request.ToDate != null)
        {
            queryParameters.Add(BuildDateQueryParameter("toDate", request.ToDate.GetValueOrDefault()));
        }

        queryParameters.AddRange(request.IsActive.Select(isActive => "isActive=" + isActive));
        queryParameters.AddRange(request.CalendarId.Select(eventType => "calendarId=" + eventType));
        queryParameters.AddRange(request.RegionId.Select(region => "regionId=" + region));
        queryParameters.AddRange(request.ShowUserEventsOnly.Select(s => "showUserEventsOnly=" + s));

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
