namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers;

public class NetworkEventsControllerTests
{
    // MFCMFC Awaiting User.GetAanMemberId() or equivalent

    // [Test, MoqAutoData]
    // public void GetCalendarEvents_ReturnsApiResponse(
    // [Frozen] Mock<IOuterApiClient> outerApiMock,
    // [Greedy] NetworkEventsController sut,
    // GetCalendarEventsQueryResult expectedResult)
    // {
    //     var request = new GetNetworkEventsRequest
    //     {
    //         Page = expectedResult.Page,
    //         PageSize = expectedResult.PageSize,
    //     };
    //
    //     //   var user = AuthenticatedUsersForTesting.FakeLocalUserFullyVerifiedClaim(apprenticeId);
    //
    //     outerApiMock.Setup(o => o.GetCalendarEvents(It.IsAny<Guid>(), It.IsAny<Dictionary<string, string[]>>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedResult);
    //     //sut.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
    //     //sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, AllNetworksUrl);
    //
    //
    //     var actualResult = sut.Index(request, new CancellationToken());
    //
    //     var viewResult = actualResult.Result.As<ViewResult>();
    //     var model = viewResult.Model as NetworkEventsViewModel;
    //     model!.TotalCount.Should().Be(expectedResult.TotalCount);
    //
    //     outerApiMock.Verify(o => o.GetCalendarEvents(It.IsAny<Guid>(), It.IsAny<Dictionary<string, string[]>>(), It.IsAny<CancellationToken>()), Times.Once);
    // }
}