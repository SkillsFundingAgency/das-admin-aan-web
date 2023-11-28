using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Aan.SharedUi.Models;
using SFA.DAS.Admin.Aan.Application.OuterApi.Calendar;
using SFA.DAS.Admin.Aan.Application.OuterApi.Calendar.Responses;
using SFA.DAS.Admin.Aan.Application.OuterApi.Regions;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Controllers;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;
using SFA.DAS.Admin.Aan.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers;

[TestFixture]

public class CalendarEventControllerTests
{
    private static readonly string NetworkEventsUrl = Guid.NewGuid().ToString();
    private static readonly string UpdateEventUrl = Guid.NewGuid().ToString();
    private static readonly string CalendarEventUrl = Guid.NewGuid().ToString();
    private static readonly string EventFormatUrl = Guid.NewGuid().ToString();
    private static readonly string EventLocationUrl = Guid.NewGuid().ToString();
    private static readonly string EventDateAndTimeUrl = Guid.NewGuid().ToString();
    private static readonly string EventDescriptionUrl = Guid.NewGuid().ToString();

    private Mock<IOuterApiClient> _outerApiMock = null!;
    private readonly Guid _calendarEventId = Guid.NewGuid();

    private static readonly string CalendarName = "cal 1";
    private static readonly int CalendarId = 1;
    private static readonly string RegionName = "East Midlands";

    private static readonly int RegionId = 1;

    [SetUp]
    public void Setup()
    {
        var calendars = new List<CalendarDetail>
        {
            new() {CalendarName= CalendarName,EffectiveFrom = DateTime.MinValue, EffectiveTo = null,Ordering = 1, Id=CalendarId}
        };
        var regionsResult = new GetRegionsResult
        {
            Regions = new List<Region>
            {
                new(RegionId, RegionName, 1)
            }
        };

        _outerApiMock = new Mock<IOuterApiClient>();
        _outerApiMock.Setup(o => o.GetCalendars(It.IsAny<CancellationToken>())).ReturnsAsync(calendars);
        _outerApiMock.Setup(o => o.GetRegions(It.IsAny<CancellationToken>())).ReturnsAsync(regionsResult);
        _outerApiMock.Setup(x => x.GetCalendarEvent(It.IsAny<Guid>(), _calendarEventId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new GetCalendarEventQueryResult
                {
                    CalendarEventId = _calendarEventId,
                    RegionName = RegionName,
                    CalendarName = CalendarName,
                    EventGuests = new List<EventGuestModel>(),
                    CalendarId = CalendarId,
                    RegionId = RegionId
                });
    }

    private Mock<IOuterApiClient> _outerApiMock = null!;
    private readonly Guid _calendarEventId = Guid.NewGuid();

    private static readonly string CalendarName = "cal 1";
    private static readonly int CalendarId = 1;
    private static readonly string RegionName = "East Midlands";

    private static readonly int RegionId = 1;

    [SetUp]
    public void Setup()
    {
        {
            new() {CalendarName= CalendarName,EffectiveFrom = DateTime.MinValue, EffectiveTo = null,Ordering = 1, Id=CalendarId}
        };
        var regionsResult = new GetRegionsResult
        {
            Regions = new List<Region>
            {
                new(RegionId, RegionName, 1)
            }
        };

        _outerApiMock = new Mock<IOuterApiClient>();
        _outerApiMock.Setup(o => o.GetCalendars(It.IsAny<CancellationToken>())).ReturnsAsync(calendars);
        _outerApiMock.Setup(o => o.GetRegions(It.IsAny<CancellationToken>())).ReturnsAsync(regionsResult);
        _outerApiMock.Setup(x => x.GetCalendarEvent(It.IsAny<Guid>(), _calendarEventId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new GetCalendarEventQueryResult
                {
                    CalendarEventId = _calendarEventId,
                    RegionName = RegionName,
                    CalendarName = CalendarName,
                    EventGuests = new List<EventGuestModel>(),
                    CalendarId = CalendarId,
                    RegionId = RegionId
                });
    }

    [Test, MoqAutoData]
    public void GetCalendarEvent_SessionModelLoaded_ReturnsExpectedViewAndModel()
    {
        var calendarName = CalendarName;
        var regionName = RegionName;

        var sessionServiceMock = new Mock<ISessionService>();
        var sessionModel = new EventSessionModel
        {
            CalendarEventId = _calendarEventId,
            CalendarName = CalendarName,
            RegionName = RegionName,
            CalendarId = CalendarId,
            RegionId = RegionId
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new CalendarEventController(_outerApiMock.Object, sessionServiceMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);

        var result = sut.Get(_calendarEventId, new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<ReviewEventViewModel>());
        var vm = actualResult.Model as ReviewEventViewModel;
        vm!.CancelLink.Should().Be(NetworkEventsUrl);
        vm.PageTitle.Should().Be(string.Empty);
        vm.EventRegion.Should().Be(regionName);
        vm.EventType.Should().Be(calendarName);
        vm.PostLink.Should().Be("#");
        vm.HasGuestSpeakersLink.Should().Be("#");
        vm.GuestSpeakersListLink.Should().Be("#");
        vm.OrganiserDetailsLink.Should().Be("#");
        vm.IsAtSchoolLink.Should().Be("#");
        vm.SchoolNameLink.Should().Be("#");
        vm.NumberOfAttendeesLink.Should().Be("#");

        _outerApiMock.Verify(x => x.GetCalendarEvent(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        sessionServiceMock.Verify(x => x.Get<EventSessionModel>(), Times.Once);
        sessionServiceMock.Verify(x => x.Set(
            It.Is<EventSessionModel>(
                s => s.HasSeenPreview == true
                     && s.IsDirectCallFromCheckYourAnswers == true
                     && s.IsAlreadyPublished == true)), Times.Once());
    }

    [Test, MoqAutoData]
    public void GetCalendarEvent_SessionModelLoadedWithNullRegionId_ReturnsExpectedViewAndModel()
    {
        const string regionName = "National";

        var sessionServiceMock = new Mock<ISessionService>();
        var sessionModel = new EventSessionModel
        {
            CalendarEventId = _calendarEventId,
            CalendarName = CalendarName,
            RegionName = RegionName,
            CalendarId = CalendarId,
            RegionId = null
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new CalendarEventController(_outerApiMock.Object, sessionServiceMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);

        var result = sut.Get(_calendarEventId, new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<ReviewEventViewModel>());
        var vm = actualResult.Model as ReviewEventViewModel;
        vm!.EventRegion.Should().Be(regionName);
    }

    [Test, MoqAutoData]
    public void GetCalendarEvent_SessionModelNotLoaded_ReturnsExpectedViewAndModel(
        Guid memberId)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns((EventSessionModel)null!);
        sessionServiceMock.Setup(x => x.GetMemberId()).Returns(memberId);

        var sut = new CalendarEventController(_outerApiMock.Object, sessionServiceMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.UpdateEvent.UpdatePreviewEvent, UpdateEventUrl);

        var result = sut.Get(_calendarEventId, new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<ReviewEventViewModel>());
        var vm = actualResult.Model as ReviewEventViewModel;
        vm!.PageTitle.Should().Be(string.Empty);
        vm.EventRegion.Should().Be(RegionName);
        vm.EventType.Should().Be(CalendarName);
        vm.PostLink.Should().Be("#");
        vm.PreviewLink.Should().Be(UpdateEventUrl);
        vm.HasGuestSpeakersLink.Should().Be("#");
        vm.GuestSpeakersListLink.Should().Be("#");
        vm.OrganiserDetailsLink.Should().Be("#");
        vm.IsAtSchoolLink.Should().Be("#");
        vm.SchoolNameLink.Should().Be("#");
        vm.NumberOfAttendeesLink.Should().Be("#");

        _outerApiMock.Verify(x => x.GetCalendarEvent(memberId, _calendarEventId, It.IsAny<CancellationToken>()), Times.Once);
        sessionServiceMock.Verify(x => x.Get<EventSessionModel>(), Times.Once);
        sessionServiceMock.Verify(x => x.Set(It.IsAny<EventSessionModel>()), Times.Once);
    }

    [Test, MoqAutoData]
    public void GetCalendarEvent_EventFormatLink_ReturnsExpectedLink(Guid memberId)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns((EventSessionModel)null!);
        sessionServiceMock.Setup(x => x.GetMemberId()).Returns(memberId);

        var sut = new CalendarEventController(_outerApiMock.Object, sessionServiceMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.UpdateEvent.UpdateEventFormat, EventFormatUrl);

        var result = sut.Get(_calendarEventId, new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<ReviewEventViewModel>());
        var vm = actualResult.Model as ReviewEventViewModel;
        vm!.EventFormatLink.Should().Be(EventFormatUrl);
    }

    [Test, MoqAutoData]
    public void GetCalendarEvent_EventLocationLink_ReturnsExpectedLink(Guid memberId)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns((EventSessionModel)null!);
        sessionServiceMock.Setup(x => x.GetMemberId()).Returns(memberId);

        var sut = new CalendarEventController(_outerApiMock.Object, sessionServiceMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.UpdateEvent.UpdateLocation, EventLocationUrl);

        var result = sut.Get(_calendarEventId, new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<ReviewEventViewModel>());
        var vm = actualResult.Model as ReviewEventViewModel;
        vm!.EventLocationLink.Should().Be(EventLocationUrl);
    }

    [Test, MoqAutoData]
    public void GetCalendarEvent_EventDateAndTimeLink_ReturnsExpectedLink(Guid memberId)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns((EventSessionModel)null!);
        sessionServiceMock.Setup(x => x.GetMemberId()).Returns(memberId);

        var sut = new CalendarEventController(_outerApiMock.Object, sessionServiceMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.UpdateEvent.UpdateDateAndTime, EventDateAndTimeUrl);

        var result = sut.Get(_calendarEventId, new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<ReviewEventViewModel>());
        var vm = actualResult.Model as ReviewEventViewModel;
        vm!.EventDateTimeLink.Should().Be(EventDateAndTimeUrl);
    }

    [Test, MoqAutoData]
    public void GetCalendarEvent_EventDescriptionLink_ReturnsExpectedLink(Guid memberId)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns((EventSessionModel)null!);
        sessionServiceMock.Setup(x => x.GetMemberId()).Returns(memberId);

        var sut = new CalendarEventController(_outerApiMock.Object, sessionServiceMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.UpdateEvent.UpdateDescription, EventDescriptionUrl);

        var result = sut.Get(_calendarEventId, new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<ReviewEventViewModel>());
        var vm = actualResult.Model as ReviewEventViewModel;
        vm!.EventDescriptionLink.Should().Be(EventDescriptionUrl);
    }

    [Test, MoqAutoData]
    public void GetCalendarEvent_EventDateAndTimeLink_ReturnsExpectedLink(Guid memberId)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns((EventSessionModel)null!);
        sessionServiceMock.Setup(x => x.GetMemberId()).Returns(memberId);

        var sut = new CalendarEventController(_outerApiMock.Object, sessionServiceMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.UpdateEvent.UpdateDateAndTime, EventDateAndTimeUrl);

        var result = sut.Get(_calendarEventId, new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<ReviewEventViewModel>());
        var vm = actualResult.Model as ReviewEventViewModel;
        vm!.EventDateTimeLink.Should().Be(EventDateAndTimeUrl);
    }

    [Test, MoqAutoData]
    public void GetCalendarEvent_EventDescriptionLink_ReturnsExpectedLink(Guid memberId)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns((EventSessionModel)null!);
        sessionServiceMock.Setup(x => x.GetMemberId()).Returns(memberId);

        var sut = new CalendarEventController(_outerApiMock.Object, sessionServiceMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.UpdateEvent.UpdateDescription, EventDescriptionUrl);

        var result = sut.Get(_calendarEventId, new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<ReviewEventViewModel>());
        var vm = actualResult.Model as ReviewEventViewModel;
        vm!.EventDescriptionLink.Should().Be(EventDescriptionUrl);
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