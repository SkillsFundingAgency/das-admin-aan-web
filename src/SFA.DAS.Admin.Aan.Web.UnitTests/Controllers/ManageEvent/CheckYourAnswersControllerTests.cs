using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Admin.Aan.Application.OuterApi.Calendar;
using SFA.DAS.Admin.Aan.Application.OuterApi.Regions;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Controllers.ManageEvent;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;
using SFA.DAS.Admin.Aan.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers.ManageEvent;
public class CheckYourAnswersControllerTests
{
    private static readonly string NetworkEventsUrl = Guid.NewGuid().ToString();
    private static readonly string PostUrl = Guid.NewGuid().ToString();

    [Test]
    [MoqAutoData]
    public void GetCheckYourAnsweers_ReturnsApiResponse(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        List<Calendar> calendars,
        GetRegionsResult regionsResult)
    {

        outerAPiMock.Setup(o => o.GetCalendars(It.IsAny<CancellationToken>())).ReturnsAsync(calendars);
        outerAPiMock.Setup(o => o.GetRegions(It.IsAny<CancellationToken>())).ReturnsAsync(regionsResult);

        var sessionServiceMock = new Mock<ISessionService>();
        var sessionModel = new EventSessionModel
        {
            CalendarId = calendars.First().Id,
            RegionId = regionsResult.Regions.First().Id
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new CheckYourAnswersController(sessionServiceMock.Object, outerAPiMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);
        var result = sut.Get(new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<CheckYourAnswersViewModel>());
        var vm = actualResult.Model as CheckYourAnswersViewModel;
        vm!.CancelLink.Should().Be(NetworkEventsUrl);
        vm.PageTitle.Should().Be(Application.Constants.CreateEvent.PageTitle);
        vm.EventRegion.Should().Be(regionsResult.Regions.First(x => x.Id == sessionModel.RegionId).Area);
        vm.EventType.Should().Be(calendars.First(x => x.Id == sessionModel.CalendarId).CalendarName);
    }

    [Test, MoqAutoData]
    public void Post_ReturnsExpectedPostLink(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        List<Calendar> calendars,
        GetRegionsResult regionsResult)
    {

        outerAPiMock.Setup(o => o.GetCalendars(It.IsAny<CancellationToken>())).ReturnsAsync(calendars);
        outerAPiMock.Setup(o => o.GetRegions(It.IsAny<CancellationToken>())).ReturnsAsync(regionsResult);

        var sessionServiceMock = new Mock<ISessionService>();
        var sessionModel = new EventSessionModel
        {
            CalendarId = calendars.First().Id,
            RegionId = regionsResult.Regions.First().Id,
            DateOfEvent = DateTime.Today,
            StartHour = DateTime.Today.Hour,
            StartMinutes = DateTime.Today.Minute,
            EndHour = DateTime.Today.Hour,
            EndMinutes = DateTime.Today.Minute
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new CheckYourAnswersController(sessionServiceMock.Object, outerAPiMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.ManageEvent.CheckYourAnswers, PostUrl);
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);

        var response = sut.Post(new CancellationToken());

        var result = response.Result as RedirectToRouteResult;
        sut.ModelState.IsValid.Should().BeTrue();
        result!.RouteName.Should().Be(RouteNames.ManageEvent.EventPublished);
    }
}
