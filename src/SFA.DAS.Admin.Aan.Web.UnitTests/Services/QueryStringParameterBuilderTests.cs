using AutoFixture.NUnit3;
using FluentAssertions;
using SFA.DAS.Admin.Aan.Domain.Constants;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvents;
using SFA.DAS.Admin.Aan.Web.Services;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Services;

public class QueryStringParameterBuilderTests
{

    [Test, AutoData]
    public void Builder_PopulatesDictionaryBuiltFromModel(string? keyword, DateTime? fromDate, DateTime? toDate,
        List<EventFormat> eventFormats, List<int> calendarIds, List<int> regionIds, int? page, int? pageSize)
    {
        var request = new GetNetworkEventsRequest
        {
            Page = page,
            PageSize = pageSize
        };

        var parameters = QueryStringParameterBuilder.BuildQueryStringParameters(request);

        parameters.TryGetValue("page", out string[]? pageResult);
        pageResult![0].Should().Be(page?.ToString());

        parameters.TryGetValue("pageSize", out string[]? pageSizeResult);
        pageSizeResult![0].Should().Be(pageSize?.ToString());
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
