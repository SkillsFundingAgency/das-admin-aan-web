using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Aan.SharedUi.Models;
using SFA.DAS.Admin.Aan.Application.OuterApi.Calendar.Responses;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Controllers;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;
using SFA.DAS.Admin.Aan.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers;

public class CalendarEventControllerTests
{
    private static readonly string NetworkEventsUrl = Guid.NewGuid().ToString();
    private static readonly string UpdateEventUrl = Guid.NewGuid().ToString();
    private static readonly string CalendarEventUrl = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void GetCalendarEvent_SessionModelLoaded_ReturnsExpectedViewAndModel(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        Guid calendarEventId)
    {
        var calendarName = "cal name";
        var regionName = "London";

        var sessionServiceMock = new Mock<ISessionService>();
        var sessionModel = new EventSessionModel
        {
            CalendarEventId = calendarEventId,
            CalendarName = calendarName,
            RegionName = regionName
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new CalendarEventController(outerAPiMock.Object, sessionServiceMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);

        var result = sut.Get(calendarEventId, new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<ReviewEventViewModel>());
        var vm = actualResult.Model as ReviewEventViewModel;
        vm!.CancelLink.Should().Be(NetworkEventsUrl);
        vm.PageTitle.Should().Be(string.Empty);
        vm.EventRegion.Should().Be(regionName);
        vm.EventType.Should().Be(calendarName);
        vm.PostLink.Should().Be("#");
        vm.EventLocationLink.Should().Be("#");
        vm.EventTypeLink.Should().Be("#");
        vm.EventDateTimeLink.Should().Be("#");
        vm.EventDescriptionLink.Should().Be("#");
        vm.HasGuestSpeakersLink.Should().Be("#");
        vm.GuestSpeakersListLink.Should().Be("#");
        vm.OrganiserDetailsLink.Should().Be("#");
        vm.IsAtSchoolLink.Should().Be("#");
        vm.SchoolNameLink.Should().Be("#");
        vm.NumberOfAttendeesLink.Should().Be("#");

        outerAPiMock.Verify(x => x.GetCalendarEvent(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        sessionServiceMock.Verify(x => x.Get<EventSessionModel>(), Times.Once);
        sessionServiceMock.Verify(x => x.Set(
            It.Is<EventSessionModel>(
                s => s.HasSeenPreview == true
                     && s.IsDirectCallFromCheckYourAnswers == true
                     && s.IsAlreadyPublished == true)), Times.Once());
    }

    [Test, MoqAutoData]
    public void GetCalendarEvent_SessionModelNotLoaded_ReturnsExpectedViewAndModel(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        Guid calendarEventId,
        Guid memberId)
    {
        var calendarName = "cal name";
        var regionName = "London";

        var sessionServiceMock = new Mock<ISessionService>();
        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns((EventSessionModel)null!);
        sessionServiceMock.Setup(x => x.GetMemberId()).Returns(memberId);

        outerAPiMock.Setup(x => x.GetCalendarEvent(memberId, calendarEventId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new GetCalendarEventQueryResult
                {
                    CalendarEventId = calendarEventId,
                    RegionName = regionName,
                    CalendarName = calendarName,
                    EventGuests = new List<EventGuestModel>(),
                    RegionId = 1
                });

        var sut = new CalendarEventController(outerAPiMock.Object, sessionServiceMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.UpdateEvent.UpdatePreviewEvent, UpdateEventUrl);

        var result = sut.Get(calendarEventId, new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<ReviewEventViewModel>());
        var vm = actualResult.Model as ReviewEventViewModel;
        vm!.PageTitle.Should().Be(string.Empty);
        vm.EventRegion.Should().Be(regionName);
        vm.EventType.Should().Be(calendarName);
        vm.PostLink.Should().Be("#");
        vm.PreviewLink.Should().Be(UpdateEventUrl);
        vm.EventLocationLink.Should().Be("#");
        vm.EventTypeLink.Should().Be("#");
        vm.EventDateTimeLink.Should().Be("#");
        vm.EventDescriptionLink.Should().Be("#");
        vm.HasGuestSpeakersLink.Should().Be("#");
        vm.GuestSpeakersListLink.Should().Be("#");
        vm.OrganiserDetailsLink.Should().Be("#");
        vm.IsAtSchoolLink.Should().Be("#");
        vm.SchoolNameLink.Should().Be("#");
        vm.NumberOfAttendeesLink.Should().Be("#");

        outerAPiMock.Verify(x => x.GetCalendarEvent(memberId, calendarEventId, It.IsAny<CancellationToken>()), Times.Once);
        sessionServiceMock.Verify(x => x.Get<EventSessionModel>(), Times.Once);
        sessionServiceMock.Verify(x => x.Set(It.IsAny<EventSessionModel>()), Times.Once);
    }

    [Test]
    public void PostCalendarEvent_RedirectToManageEvents()
    {
        var sut = new CalendarEventController(Mock.Of<IOuterApiClient>(), Mock.Of<ISessionService>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);
        var result = (RedirectToRouteResult)sut.Post();

        sut.ModelState.IsValid.Should().BeTrue();
        result.RouteName.Should().Be(RouteNames.NetworkEvents);
    }

    [Test, MoqAutoData]
    public void GetCalendarEventPreview_SessionModelLoaded_ReturnsExpectedViewAndModel(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        Guid calendarEventId)
    {
        var calendarName = "cal name";
        var regionName = "London";

        var sessionServiceMock = new Mock<ISessionService>();
        var sessionModel = new EventSessionModel
        {
            CalendarEventId = calendarEventId,
            CalendarName = calendarName,
            RegionName = regionName
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new CalendarEventController(outerAPiMock.Object, sessionServiceMock.Object);
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CalendarEvent, CalendarEventUrl);

        var result = sut.GetPreview();
        var actualResult = result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<NetworkEventDetailsViewModel>());
        var vm = actualResult.Model as NetworkEventDetailsViewModel;
        vm!.BackLinkUrl.Should().Be(CalendarEventUrl);
        sessionServiceMock.Verify(x => x.Get<EventSessionModel>(), Times.Once);
    }
}