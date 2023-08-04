using AutoFixture.NUnit3;
using FluentAssertions;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvents;
using SFA.DAS.Admin.Aan.Web.Services;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Services;

public class QueryStringParameterBuilderTests
{

    [Test, AutoData]
    public void Builder_PopulatesDictionaryBuiltFromModel(DateTime? fromDate, DateTime? toDate, bool? isActive, List<int> calendarIds, int? page, int? pageSize)
    {
        var request = new GetNetworkEventsRequest
        {
            FromDate = fromDate,
            ToDate = toDate,
            CalendarId = calendarIds,
            Page = page,
            PageSize = pageSize
        };

        if (isActive.HasValue)
        {
            request.IsActive = new List<bool> { isActive.Value };
        }

        var parameters = QueryStringParameterBuilder.BuildQueryStringParameters(request);

        parameters.TryGetValue("fromDate", out string[]? fromDateResult);
        fromDateResult![0].Should().Be(fromDate?.ToString("yyyy-MM-dd"));

        parameters.TryGetValue("toDate", out var toDateResult);
        toDateResult![0].Should().Be(toDate?.ToString("yyyy-MM-dd"));

        if (isActive.HasValue)
        {
            parameters.TryGetValue("isActive", out var isActiveResult);
            isActiveResult!.Length.Should().Be(1);
            isActive.Value.ToString().Should().Be(isActiveResult.First().ToString());

        }
        parameters.TryGetValue("calendarId", out var calendarIdsResult);
        calendarIdsResult!.Length.Should().Be(calendarIds.Count);
        calendarIds.Select(x => x.ToString()).Should().BeEquivalentTo(calendarIdsResult.ToList());

        parameters.TryGetValue("page", out string[]? pageResult);
        pageResult![0].Should().Be(page?.ToString());

        parameters.TryGetValue("pageSize", out string[]? pageSizeResult);
        pageSizeResult![0].Should().Be(pageSize?.ToString());
    }

    [TestCase(null)]
    [TestCase("2030-06-01")]
    public void Builder_ConstructParameters_FromDate(DateTime? fromDate)
    {
        var parameters = QueryStringParameterBuilder.BuildQueryStringParameters(new GetNetworkEventsRequest
        {
            FromDate = fromDate
        });
        parameters.TryGetValue("fromDate", out var fromDateResult);
        if (fromDate.HasValue)
        {
            fromDateResult![0].Should().Be(fromDate?.ToString("yyyy-MM-dd"));
        }
        else
        {
            fromDateResult.Should().BeNull();
        }
    }

    [TestCase(null)]
    [TestCase("2030-06-01")]
    public void Builder_ConstructParameters_ToDate(DateTime? toDate)
    {
        var parameters = QueryStringParameterBuilder.BuildQueryStringParameters(new GetNetworkEventsRequest
        {
            ToDate = toDate
        });
        parameters.TryGetValue("toDate", out var toDateResult);
        if (toDate.HasValue)
        {
            toDateResult![0].Should().Be(toDate?.ToString("yyyy-MM-dd"));
        }
        else
        {
            toDateResult.Should().BeNull();
        }
    }

    [TestCase(null)]
    [TestCase(true)]
    [TestCase(false)]
    public void Builder_ConstructParameters_EventStatus(bool? isActive)
    {
        var request = new GetNetworkEventsRequest();
        if (isActive.HasValue)
        {
            request.IsActive = new List<bool> { isActive.Value };
        }

        var parameters = QueryStringParameterBuilder.BuildQueryStringParameters(request);
        parameters.TryGetValue("isActive", out var eventStatusResult);
        if (isActive.HasValue)
        {
            eventStatusResult![0].Should().Be(isActive.ToString());
        }
        else
        {
            eventStatusResult.Should().BeNull();
        }
    }

    [Test]
    public void Builder_ConstructParameters_ToCalendarId([ValueSource(nameof(NullableIntRange))] int? calendarId)
    {
        var request = new GetNetworkEventsRequest();
        if (calendarId.HasValue)
        {
            request.CalendarId = new List<int> { calendarId.Value };
        }

        var parameters = QueryStringParameterBuilder.BuildQueryStringParameters(request);

        parameters.TryGetValue("calendarId", out var calendarIdResult);
        if (calendarId.HasValue)
        {
            calendarIdResult![0].Should().Be(calendarId?.ToString());
        }
        else
        {
            calendarIdResult.Should().BeEmpty();
        }
    }

    [Test]
    public void Builder_ConstructParameters_ToRegionId([ValueSource(nameof(NullableIntRange))] int? regionId)
    {
        var request = new GetNetworkEventsRequest();
        if (regionId.HasValue)
        {
            request.RegionId = new List<int> { regionId.Value };
        }

        var parameters = QueryStringParameterBuilder.BuildQueryStringParameters(request);

        parameters.TryGetValue("regionId", out var regionIdResult);
        if (regionId.HasValue)
        {
            regionIdResult![0].Should().Be(regionId?.ToString());
        }
        else
        {
            regionIdResult.Should().BeEmpty();
        }
    }

    [Test]
    public void Builder_ConstructParameters_ToPage([ValueSource(nameof(NullableIntRange))] int? page)
    {
        var parameters = QueryStringParameterBuilder.BuildQueryStringParameters(new GetNetworkEventsRequest
        {
            Page = page
        });

        parameters.TryGetValue("page", out var pageResult);
        if (page.HasValue)
        {
            pageResult![0].Should().Be(page?.ToString());
        }
        else
        {
            pageResult.Should().BeNull();
        }
    }

    [Test]
    public void Builder_ConstructParameters_ToPageSize([ValueSource(nameof(NullableIntRange))] int? pageSize)
    {
        var parameters = QueryStringParameterBuilder.BuildQueryStringParameters(new GetNetworkEventsRequest
        {
            PageSize = pageSize
        });

        parameters.TryGetValue("pageSize", out var pageSizeResult);
        if (pageSize.HasValue)
        {
            pageSizeResult![0].Should().Be(pageSize?.ToString());
        }
        else
        {
            pageSizeResult.Should().BeNull();
        }
    }

    [Test, AutoData]
    public void Builder_PopulatesParametersFromRequestWithNullPageAndPageSize()
    {
        var request = new GetNetworkEventsRequest();

        var parameters = QueryStringParameterBuilder.BuildQueryStringParameters(request);

        parameters.ContainsKey("page").Should().BeFalse();
        parameters.ContainsKey("pageSize").Should().BeFalse();
    }

    private static readonly int?[] NullableIntRange = { null, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
}
