using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Domain.OuterApi.Responses;
using SFA.DAS.Admin.Aan.Web.Controllers;
using SFA.DAS.Admin.Aan.Web.Extensions;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvents;
using SFA.DAS.Admin.Aan.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers;

public class NetworkEventsControllerTests
{
    private static readonly string AllNetworksUrl = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void GetCalendarEvents_ReturnsApiResponse(
    [Frozen] Mock<IOuterApiClient> outerApiMock,
    [Greedy] NetworkEventsController sut,
    GetCalendarEventsQueryResult expectedResult,
    DateTime? fromDate,
    DateTime? toDate)
    {
        var fromDateFormatted = fromDate?.ToString("yyyy-MM-dd")!;
        var toDateFormatted = toDate?.ToString("yyyy-MM-dd")!;

        var request = new GetNetworkEventsRequest
        {
            Page = expectedResult.Page,
            PageSize = expectedResult.PageSize,
        };

        outerApiMock.Setup(o => o.GetCalendarEvents(It.IsAny<Guid>(), It.IsAny<Dictionary<string, string[]>>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedResult);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, AllNetworksUrl);

        var actualResult = sut.Index(request, new CancellationToken());

        var viewResult = actualResult.Result.As<ViewResult>();
        var model = viewResult.Model as NetworkEventsViewModel;
        model!.PaginationViewModel.CurrentPage.Should().Be(expectedResult.Page);
        model!.PaginationViewModel.PageSize.Should().Be(expectedResult.PageSize);
        model!.PaginationViewModel.TotalPages.Should().Be(expectedResult.TotalPages);
        model!.TotalCount.Should().Be(expectedResult.TotalCount);
        model.FilterChoices.FromDate?.ToApiString().Should().Be(fromDateFormatted);
        model.FilterChoices.ToDate?.ToApiString().Should().Be(toDateFormatted);

        outerApiMock.Verify(o => o.GetCalendarEvents(It.IsAny<Guid>(), It.IsAny<Dictionary<string, string[]>>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}