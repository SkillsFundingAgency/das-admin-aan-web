using AutoFixture.NUnit3;
using FluentAssertions;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvents;
using SFA.DAS.Admin.Aan.Web.Services;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Services;

public class QueryStringParameterBuilderTests
{

    [Test, AutoData]
    public void Builder_PopulatesDictionaryBuiltFromModel(DateTime? fromDate, DateTime? toDate, int? page, int? pageSize)
    {
        var request = new GetNetworkEventsRequest
        {
            FromDate = fromDate,
            ToDate = toDate,
            Page = page,
            PageSize = pageSize
        };

        var parameters = QueryStringParameterBuilder.BuildQueryStringParameters(request);

        parameters.TryGetValue("fromDate", out string[]? fromDateResult);
        fromDateResult![0].Should().Be(fromDate?.ToString("yyyy-MM-dd"));

        parameters.TryGetValue("toDate", out var toDateResult);
        toDateResult![0].Should().Be(toDate?.ToString("yyyy-MM-dd"));

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
        if (fromDate != null)
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
        if (toDate != null)
        {
            toDateResult![0].Should().Be(toDate?.ToString("yyyy-MM-dd"));
        }
        else
        {
            toDateResult.Should().BeNull();
        }
    }

    [TestCase(null)]
    [TestCase(3)]
    public void Builder_ConstructParameters_ToPage(int? page)
    {
        var parameters = QueryStringParameterBuilder.BuildQueryStringParameters(new GetNetworkEventsRequest
        {
            Page = page
        });

        parameters.TryGetValue("page", out var pageResult);
        if (pageResult != null)
        {
            pageResult![0].Should().Be(page?.ToString());
        }
        else
        {
            pageResult.Should().BeNull();
        }
    }

    [TestCase(null)]
    [TestCase(6)]
    public void Builder_ConstructParameters_ToPageSize(int? pageSize)
    {
        var parameters = QueryStringParameterBuilder.BuildQueryStringParameters(new GetNetworkEventsRequest
        {
            PageSize = pageSize
        });

        parameters.TryGetValue("pageSize", out var pageSizeResult);
        if (pageSizeResult != null)
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
}
