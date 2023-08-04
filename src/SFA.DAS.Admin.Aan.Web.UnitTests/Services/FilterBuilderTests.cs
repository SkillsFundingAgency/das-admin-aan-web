using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvents;
using SFA.DAS.Admin.Aan.Web.Services;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Services;

[TestFixture]
public class FilterBuilderTests
{
    private const string LocationUrl = "network-events";

    [Test]
    public void BuildFilterChoicesForNoFilters()
    {

        var mockUrlHelper = new Mock<IUrlHelper>();
        mockUrlHelper
            .Setup(x => x.RouteUrl(It.IsAny<UrlRouteContext>()))
            .Returns(LocationUrl);

        var request = new GetNetworkEventsRequest
        {
            FromDate = null,
            ToDate = null
        };


        var actual = FilterBuilder.Build(request, mockUrlHelper.Object, new List<ChecklistLookup>(), new List<ChecklistLookup>());
        actual.Count.Should().Be(0);
    }

    [TestCase(null, "", 0)]
    [TestCase("2023-05-31", "From date", 1)]
    public void BuildEventSearchFiltersForFromDate(DateTime? fromDate, string fieldName1, int expectedNumberOfFilters)
    {
        var mockUrlHelper = new Mock<IUrlHelper>();
        mockUrlHelper
            .Setup(x => x.RouteUrl(It.IsAny<UrlRouteContext>()))
            .Returns(LocationUrl);

        var request = new GetNetworkEventsRequest
        {
            FromDate = fromDate
        };

        var actual = FilterBuilder.Build(request, mockUrlHelper.Object, new List<ChecklistLookup>(), new List<ChecklistLookup>());
        actual.Count.Should().Be(expectedNumberOfFilters);
        if (expectedNumberOfFilters > 0)
        {
            var firstItem = actual.First();
            firstItem.FieldName.Should().Be(fieldName1);
            firstItem.FieldOrder.Should().Be(1);
            firstItem.Filters.First().ClearFilterLink.Should().Be(LocationUrl);
            firstItem.Filters.First().Order.Should().Be(1);
            if (fieldName1 != "")
            {
                firstItem.Filters.First().Value.Should().Be(fromDate?.ToString("dd/MM/yyyy"));
            }
        }
    }

    [TestCase(null, "", 0)]
    [TestCase("2024-01-01", "To date", 1)]
    public void BuildEventSearchFiltersForToDate(DateTime? toDate, string fieldName1, int expectedNumberOfFilters)
    {
        var mockUrlHelper = new Mock<IUrlHelper>();
        mockUrlHelper
            .Setup(x => x.RouteUrl(It.IsAny<UrlRouteContext>()))
            .Returns(LocationUrl);

        var request = new GetNetworkEventsRequest
        {
            ToDate = toDate
        };

        var actual = FilterBuilder.Build(request, mockUrlHelper.Object, new List<ChecklistLookup>(), new List<ChecklistLookup>());

        actual.Count.Should().Be(expectedNumberOfFilters);
        if (expectedNumberOfFilters > 0)
        {
            var firstItem = actual.First();
            firstItem.FieldName.Should().Be(fieldName1);
            firstItem.FieldOrder.Should().Be(1);
            firstItem.Filters.First().ClearFilterLink.Should().Be(LocationUrl);
            firstItem.Filters.First().Order.Should().Be(1);
            if (fieldName1 != "")
            {
                firstItem.Filters.First().Value.Should().Be(toDate?.ToString("dd/MM/yyyy"));
            }
        }
    }

    [TestCase("2023-05-31", "2024-01-01", "?toDate=2024-01-01", "?fromDate=2023-05-31", "From date", "To date")]
    public void BuildEventSearchFiltersForFromDateAndToDate(DateTime? fromDate, DateTime? toDate, string expectedFirst, string expectedSecond, string fieldName1, string fieldName2)
    {
        var mockUrlHelper = new Mock<IUrlHelper>();
        mockUrlHelper
            .Setup(x => x.RouteUrl(It.IsAny<UrlRouteContext>()))
            .Returns(LocationUrl);


        var request = new GetNetworkEventsRequest
        {
            FromDate = fromDate,
            ToDate = toDate
        };

        var actual = FilterBuilder.Build(request, mockUrlHelper.Object, new List<ChecklistLookup>(), new List<ChecklistLookup>());

        actual.Count.Should().Be(2);

        var firstItem = actual.First();
        firstItem.FieldName.Should().Be(fieldName1);
        firstItem.FieldOrder.Should().Be(1);
        firstItem.Filters.First().ClearFilterLink.Should().Be(LocationUrl + expectedFirst);
        firstItem.Filters.First().Order.Should().Be(1);
        if (fieldName1 == "From date")
        {
            firstItem.Filters.First().Value.Should().Be(fromDate?.ToString("dd/MM/yyyy"));
        }
        else if (fieldName1 == "To date")
        {
            firstItem.Filters.First().Value.Should().Be(toDate?.ToString("dd/MM/yyyy"));
        }

        var secondItem = actual.Skip(1).First();

        secondItem.FieldName.Should().Be(fieldName2);
        secondItem.FieldOrder.Should().Be(2);
        secondItem.Filters.First().ClearFilterLink.Should().Be(LocationUrl + expectedSecond);
        secondItem.Filters.First().Order.Should().Be(1);
        if (fieldName2 == "To date")
        {
            secondItem.Filters.First().Value.Should().Be(toDate?.ToString("dd/MM/yyyy"));
        }
    }

    [TestCase(null, "", 0, null)]
    [TestCase(true, "Event status", 1, "Published")]
    [TestCase(false, "Event status", 1, "Cancelled")]
    public void BuildEventSearchFiltersForEventStatus(bool? eventStatus, string fieldName1, int expectedNumberOfFilters, string? expectedValue)
    {
        var mockUrlHelper = new Mock<IUrlHelper>();
        mockUrlHelper
            .Setup(x => x.RouteUrl(It.IsAny<UrlRouteContext>()))
            .Returns(LocationUrl);

        var request = new GetNetworkEventsRequest();
        if (eventStatus.HasValue)
        {
            request.IsActive = new List<bool> { eventStatus.Value };
        }

        var actual = FilterBuilder.Build(request, mockUrlHelper.Object, ChecklistLookupEventStatus(), new List<ChecklistLookup>());
        actual.Count.Should().Be(expectedNumberOfFilters);
        if (expectedNumberOfFilters > 0)
        {
            var firstItem = actual.First();
            firstItem.FieldName.Should().Be(fieldName1);
            firstItem.FieldOrder.Should().Be(1);
            firstItem.Filters.First().ClearFilterLink.Should().Be(LocationUrl);
            firstItem.Filters.First().Order.Should().Be(1);
            if (fieldName1 != "")
            {
                firstItem.Filters.First().Value.Should().Be(expectedValue);
            }
        }
    }

    [TestCase(null, "", 0)]
    [TestCase(1, "Event type", 1)]
    [TestCase(2, "Event type", 1)]
    [TestCase(3, "Event type", 1)]
    public void BuildEventSearchFiltersForEventTypes(int? calendarId, string fieldName, int expectedNumberOfFilters)
    {
        var parameterName = "calendarId";
        var request = new GetNetworkEventsRequest { CalendarId = new List<int>() };
        var eventTypesLookup = new List<ChecklistLookup>();

        var eventFilters = new GetNetworkEventsRequest
        {
            CalendarId = new List<int>()
        };

        if (calendarId != null)
        {
            eventTypesLookup.Add(new ChecklistLookup(parameterName, calendarId.Value.ToString()));
            eventFilters.CalendarId.Add(calendarId.Value);
            request.CalendarId.Add(calendarId.Value);
        }

        var mockUrlHelper = new Mock<IUrlHelper>();
        mockUrlHelper
            .Setup(x => x.RouteUrl(It.IsAny<UrlRouteContext>()))
            .Returns(LocationUrl);

        var actual = FilterBuilder.Build(request, mockUrlHelper.Object, new List<ChecklistLookup>(), eventTypesLookup);

        if (expectedNumberOfFilters == 0)
        {
            actual.Count.Should().Be(0);
            return;
        }

        var firstItem = actual.First();
        firstItem.Filters.Count.Should().Be(expectedNumberOfFilters);
        firstItem.FieldName.Should().Be(fieldName);
        firstItem.FieldOrder.Should().Be(1);

        var filter = firstItem.Filters.First();
        filter.ClearFilterLink.Should().Be(LocationUrl);
        filter.Order.Should().Be(1);
        filter.Value.Should().Be(parameterName);
    }



    [TestCase(1, 2, "?calendarId=2", "?calendarId=1")]
    [TestCase(1, 3, "?calendarId=3", "?calendarId=1")]
    [TestCase(2, 3, "?calendarId=3", "?calendarId=2")]
    public void BuildEventSearchFiltersForTwoEventTypes(int calendarId1, int calendarId2,
      string expectedFirst, string expectedSecond)
    {
        var parameterName = "calendarId";
        var request = new GetNetworkEventsRequest { CalendarId = new List<int>() };
        var eventTypesLookup = new List<ChecklistLookup>();

        var eventFilters = new GetNetworkEventsRequest
        {
            CalendarId = new List<int>()
        };

        eventTypesLookup.Add(new ChecklistLookup(parameterName, calendarId1.ToString()));
        eventFilters.CalendarId.Add(calendarId1);
        request.CalendarId.Add(calendarId1);

        eventTypesLookup.Add(new ChecklistLookup(parameterName, calendarId2.ToString()));
        eventFilters.CalendarId.Add(calendarId2);
        request.CalendarId.Add(calendarId2);

        var mockUrlHelper = new Mock<IUrlHelper>();
        mockUrlHelper
            .Setup(x => x.RouteUrl(It.IsAny<UrlRouteContext>()))
            .Returns(LocationUrl);

        var actual = FilterBuilder.Build(request, mockUrlHelper.Object, new List<ChecklistLookup>(), eventTypesLookup);

        var firstItem = actual.First();
        firstItem.Filters.Count.Should().Be(2);
        firstItem.FieldName.Should().Be("Event type");
        firstItem.FieldOrder.Should().Be(1);

        var filter = firstItem.Filters.First();
        filter.ClearFilterLink.Should().Be(LocationUrl + expectedFirst);
        filter.Order.Should().Be(1);
        filter.Value.Should().Be(parameterName);


        var filterSecond = firstItem.Filters.Skip(1).First();
        filterSecond.ClearFilterLink.Should().Be(LocationUrl + expectedSecond);
        filterSecond.Order.Should().Be(2);
        filterSecond.Value.Should().Be(parameterName);
    }

    [TestCase(1, 2, 3, "?calendarId=2&calendarId=3", "?calendarId=1&calendarId=3", "?calendarId=1&calendarId=2")]
    public void BuildEventSearchFiltersForThreeEventTypes(int calendarId1, int calendarId2, int calendarId3, string expectedFirst, string expectedSecond, string expectedThird)
    {
        var parameterName = "calendarId";
        var request = new GetNetworkEventsRequest { CalendarId = new List<int>() };
        var eventTypesLookup = new List<ChecklistLookup>();

        var eventFilters = new GetNetworkEventsRequest
        {
            CalendarId = new List<int>()
        };

        eventTypesLookup.Add(new ChecklistLookup(parameterName, calendarId1.ToString()));
        eventFilters.CalendarId.Add(calendarId1);
        request.CalendarId.Add(calendarId1);

        eventTypesLookup.Add(new ChecklistLookup(parameterName, calendarId2.ToString()));
        eventFilters.CalendarId.Add(calendarId2);
        request.CalendarId.Add(calendarId2);

        eventTypesLookup.Add(new ChecklistLookup(parameterName, calendarId3.ToString()));
        eventFilters.CalendarId.Add(calendarId3);
        request.CalendarId.Add(calendarId3);

        var mockUrlHelper = new Mock<IUrlHelper>();
        mockUrlHelper
            .Setup(x => x.RouteUrl(It.IsAny<UrlRouteContext>()))
            .Returns(LocationUrl);

        var actual = FilterBuilder.Build(request, mockUrlHelper.Object, new List<ChecklistLookup>(), eventTypesLookup);

        var firstItem = actual.First();
        firstItem.Filters.Count.Should().Be(3);
        firstItem.FieldName.Should().Be("Event type");
        firstItem.FieldOrder.Should().Be(1);

        var filter = firstItem.Filters.First();
        filter.ClearFilterLink.Should().Be(LocationUrl + expectedFirst);
        filter.Order.Should().Be(1);
        filter.Value.Should().Be(parameterName);

        var filterSecond = firstItem.Filters.Skip(1).First();
        filterSecond.ClearFilterLink.Should().Be(LocationUrl + expectedSecond);
        filterSecond.Order.Should().Be(2);
        filterSecond.Value.Should().Be(parameterName);

        var filterThird = firstItem.Filters.Skip(2).First();
        filterThird.ClearFilterLink.Should().Be(LocationUrl + expectedThird);
        filterThird.Order.Should().Be(3);
        filterThird.Value.Should().Be(parameterName);
    }

    private static List<ChecklistLookup> ChecklistLookupEventStatus() =>
        new()
        {
            new ChecklistLookup("Published", true.ToString()),
            new ChecklistLookup("Cancelled", false.ToString())
        };
}