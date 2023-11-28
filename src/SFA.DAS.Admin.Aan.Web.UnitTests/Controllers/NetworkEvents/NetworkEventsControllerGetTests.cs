using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Admin.Aan.Application.OuterApi.Calendar;
using SFA.DAS.Admin.Aan.Application.OuterApi.Calendar.Responses;
using SFA.DAS.Admin.Aan.Application.OuterApi.Regions;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Controllers;
using SFA.DAS.Admin.Aan.Web.Extensions;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.DeleteEvent;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvents;
using SFA.DAS.Admin.Aan.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers.NetworkEvents;

public class NetworkEventsControllerGetTests
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

        outerApiMock
            .Setup(o => o.GetCalendarEvents(It.IsAny<Guid>(), It.IsAny<Dictionary<string, string[]>>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(expectedResult);

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

        outerApiMock.Verify(
            o => o.GetCalendarEvents(It.IsAny<Guid>(), It.IsAny<Dictionary<string, string[]>>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }


    [TestCase(true, true)]
    [TestCase(true, false)]
    [TestCase(false, true)]
    [TestCase(false, false)]

    public void GetCalendarEventsWithEventStatusCheck_ReturnsApiResponse(bool isPublishedTicked, bool isCancelledTicked)
    {

        var request = new GetNetworkEventsRequest
        {
            Page = 1,
            PageSize = 1
        };

        if (isPublishedTicked)
            request.IsActive.Add(true);
        if (isCancelledTicked)
            request.IsActive.Add(false);


        var expectedResult = new GetCalendarEventsQueryResult();

        var outerApiMock = new Mock<IOuterApiClient>();
        var sessionServiceMock = new Mock<ISessionService>();
        outerApiMock
            .Setup(o => o.GetCalendarEvents(It.IsAny<Guid>(), It.IsAny<Dictionary<string, string[]>>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(expectedResult);
        outerApiMock.Setup(o => o.GetCalendars(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Calendar>());
        outerApiMock.Setup(o => o.GetRegions(It.IsAny<CancellationToken>())).ReturnsAsync(new GetRegionsResult());

        var sut = new NetworkEventsController(outerApiMock.Object, sessionServiceMock.Object, Mock.Of<IValidator<CancelEventViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, AllNetworksUrl);

        var actualResult = sut.Index(request, new CancellationToken());

        var viewResult = actualResult.Result.As<ViewResult>();
        var model = viewResult.Model as NetworkEventsViewModel;

        model!.ClearSelectedFiltersLink.Should().Be(AllNetworksUrl);

        switch (isPublishedTicked)
        {
            case true:
                model!.FilterChoices.EventStatusChecklistDetails.Lookups.First(x => x.Name == "Published").Checked
                    .Should().NotBeEmpty();
                break;
            case false:
                model!.FilterChoices.EventStatusChecklistDetails.Lookups.First(x => x.Name == "Published").Checked
                    .Should().BeEmpty();
                break;
        }

        switch (isCancelledTicked)
        {
            case true:
                model!.FilterChoices.EventStatusChecklistDetails.Lookups.First(x => x.Name == "Cancelled").Checked
                    .Should().NotBeEmpty();
                break;
            case false:
                model!.FilterChoices.EventStatusChecklistDetails.Lookups.First(x => x.Name == "Cancelled").Checked
                    .Should().BeEmpty();
                break;
        }

        sessionServiceMock.Verify(s => s.Delete(nameof(EventSessionModel)), Times.Once);
    }

    [Test]
    public void CreateEvent_BuildSessionModelAndRerouteToCreateEventFormat()
    {
        var sessionServiceMock = new Mock<ISessionService>();

        var sut = new NetworkEventsController(Mock.Of<IOuterApiClient>(), sessionServiceMock.Object, Mock.Of<IValidator<CancelEventViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.CreateNewEvent, AllNetworksUrl);

        var actualResult = sut.CreateEvent();

        var result = (RedirectToRouteResult)actualResult;

        sessionServiceMock.Verify(s => s.Set(It.IsAny<EventSessionModel>()), Times.Once());
        result.RouteName.Should().Be(RouteNames.CreateEvent.EventFormat);
    }
}