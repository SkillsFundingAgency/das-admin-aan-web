using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Domain.OuterApi.Responses;
using SFA.DAS.Admin.Aan.Web.Controllers;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvents;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers;

public class NetworkEventsControllerTests
{


    [Test, MoqAutoData]
    public void GetCalendarEvents_ReturnsApiResponse(
    [Frozen] Mock<IOuterApiClient> outerApiMock,
    [Greedy] NetworkEventsController sut,
    GetCalendarEventsQueryResult expectedResult)
    {
        var request = new GetNetworkEventsRequest
        {
            Page = expectedResult.Page,
            PageSize = expectedResult.PageSize,
        };

        //   var user = AuthenticatedUsersForTesting.FakeLocalUserFullyVerifiedClaim(apprenticeId);

        outerApiMock.Setup(o => o.GetCalendarEvents(It.IsAny<Guid>(), It.IsAny<Dictionary<string, string[]>>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedResult);
        //sut.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
        //sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, AllNetworksUrl);


        var actualResult = sut.Index(request, new CancellationToken());

        var viewResult = actualResult.Result.As<ViewResult>();
        var model = viewResult.Model as NetworkEventsViewModel;
        model!.TotalCount.Should().Be(expectedResult.TotalCount);

        outerApiMock.Verify(o => o.GetCalendarEvents(It.IsAny<Guid>(), It.IsAny<Dictionary<string, string[]>>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}