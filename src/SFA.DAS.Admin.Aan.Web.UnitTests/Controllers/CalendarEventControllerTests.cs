using AutoFixture.NUnit3;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Aan.SharedUi.Constants;
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
    private static readonly string HasGuestSpeakersUrl = Guid.NewGuid().ToString();
    private static readonly string ListGuestSpeakersUrl = Guid.NewGuid().ToString();
    private static readonly string OrganiserDetailsUrl = Guid.NewGuid().ToString();
    private static readonly string NumberOfAttendeesUrl = Guid.NewGuid().ToString();
    private static readonly string IsAtSchoolUrl = Guid.NewGuid().ToString();
    private static readonly string SchoolNameUrl = Guid.NewGuid().ToString();
    private static readonly string NotifyAttendeesUrl = Guid.NewGuid().ToString();

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
            new() { CalendarName = CalendarName, EffectiveFrom = DateTime.MinValue, EffectiveTo = null, Ordering = 1, Id = CalendarId }
        };
        var regionsResult = new GetRegionsResult
        {
            Regions = [new(RegionId, RegionName, 1)]
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

        var sut = new CalendarEventController(_outerApiMock.Object, sessionServiceMock.Object, Mock.Of<IValidator<ReviewEventViewModel>>());

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

        var sut = new CalendarEventController(_outerApiMock.Object, sessionServiceMock.Object, Mock.Of<IValidator<ReviewEventViewModel>>());

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

        var sut = new CalendarEventController(_outerApiMock.Object, sessionServiceMock.Object, Mock.Of<IValidator<ReviewEventViewModel>>());

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

        var sut = new CalendarEventController(_outerApiMock.Object, sessionServiceMock.Object, Mock.Of<IValidator<ReviewEventViewModel>>());

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

        var sut = new CalendarEventController(_outerApiMock.Object, sessionServiceMock.Object, Mock.Of<IValidator<ReviewEventViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.UpdateEvent.UpdateLocation, EventLocationUrl);

        var result = sut.Get(_calendarEventId, new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<ReviewEventViewModel>());
        var vm = actualResult.Model as ReviewEventViewModel;
        vm!.EventLocationLink.Should().Be(EventLocationUrl);
    }

    [Test, MoqAutoData]
    public void GetCalendarEvent_HasGuestSpeakersLink_ReturnsExpectedLink(Guid memberId)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns((EventSessionModel)null!);
        sessionServiceMock.Setup(x => x.GetMemberId()).Returns(memberId);

        var sut = new CalendarEventController(_outerApiMock.Object, sessionServiceMock.Object, Mock.Of<IValidator<ReviewEventViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.UpdateEvent.UpdateHasGuestSpeakers, HasGuestSpeakersUrl);

        var result = sut.Get(_calendarEventId, new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<ReviewEventViewModel>());
        var vm = actualResult.Model as ReviewEventViewModel;
        vm!.HasGuestSpeakersLink.Should().Be(HasGuestSpeakersUrl);
    }

    [Test, MoqAutoData]
    public void GetCalendarEvent_GuestSpeakerListLink_ReturnsExpectedLink(Guid memberId)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns((EventSessionModel)null!);
        sessionServiceMock.Setup(x => x.GetMemberId()).Returns(memberId);

        var sut = new CalendarEventController(_outerApiMock.Object, sessionServiceMock.Object, Mock.Of<IValidator<ReviewEventViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.UpdateEvent.UpdateGuestSpeakerList, ListGuestSpeakersUrl);

        var result = sut.Get(_calendarEventId, new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<ReviewEventViewModel>());
        var vm = actualResult.Model as ReviewEventViewModel;
        vm!.GuestSpeakersListLink.Should().Be(ListGuestSpeakersUrl);
    }

    [Test, MoqAutoData]
    public void GetCalendarEvent_EventDateAndTimeLink_ReturnsExpectedLink(Guid memberId)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns((EventSessionModel)null!);
        sessionServiceMock.Setup(x => x.GetMemberId()).Returns(memberId);

        var sut = new CalendarEventController(_outerApiMock.Object, sessionServiceMock.Object, Mock.Of<IValidator<ReviewEventViewModel>>());

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

        var sut = new CalendarEventController(_outerApiMock.Object, sessionServiceMock.Object, Mock.Of<IValidator<ReviewEventViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.UpdateEvent.UpdateDescription, EventDescriptionUrl);

        var result = sut.Get(_calendarEventId, new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<ReviewEventViewModel>());
        var vm = actualResult.Model as ReviewEventViewModel;
        vm!.EventDescriptionLink.Should().Be(EventDescriptionUrl);
    }

    [Test, MoqAutoData]
    public void GetCalendarEvent_OrganiserDetailsLink_ReturnsExpectedLink(Guid memberId)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns((EventSessionModel)null!);
        sessionServiceMock.Setup(x => x.GetMemberId()).Returns(memberId);

        var sut = new CalendarEventController(_outerApiMock.Object, sessionServiceMock.Object, Mock.Of<IValidator<ReviewEventViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.UpdateEvent.UpdateOrganiserDetails, OrganiserDetailsUrl);

        var result = sut.Get(_calendarEventId, new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<ReviewEventViewModel>());
        var vm = actualResult.Model as ReviewEventViewModel;
        vm!.OrganiserDetailsLink.Should().Be(OrganiserDetailsUrl);
    }

    [Test, MoqAutoData]
    public void GetCalendarEvent_NumberOfAttendeesLink_ReturnsExpectedLink(Guid memberId)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns((EventSessionModel)null!);
        sessionServiceMock.Setup(x => x.GetMemberId()).Returns(memberId);

        var sut = new CalendarEventController(_outerApiMock.Object, sessionServiceMock.Object, Mock.Of<IValidator<ReviewEventViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.UpdateEvent.UpdateNumberOfAttendees, NumberOfAttendeesUrl);

        var result = sut.Get(_calendarEventId, new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<ReviewEventViewModel>());
        var vm = actualResult.Model as ReviewEventViewModel;
        vm!.NumberOfAttendeesLink.Should().Be(NumberOfAttendeesUrl);
    }

    [Test, MoqAutoData]
    public void GetCalendarEvent_IsAtSchoolLink_ReturnsExpectedLink(Guid memberId)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns((EventSessionModel)null!);
        sessionServiceMock.Setup(x => x.GetMemberId()).Returns(memberId);

        var sut = new CalendarEventController(_outerApiMock.Object, sessionServiceMock.Object, Mock.Of<IValidator<ReviewEventViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.UpdateEvent.UpdateIsAtSchool, IsAtSchoolUrl);

        var result = sut.Get(_calendarEventId, new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<ReviewEventViewModel>());
        var vm = actualResult.Model as ReviewEventViewModel;
        vm!.IsAtSchoolLink.Should().Be(IsAtSchoolUrl);
    }


    [Test, MoqAutoData]
    public void GetCalendarEvent_SchoolNameLink_ReturnsExpectedLink(Guid memberId)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns((EventSessionModel)null!);
        sessionServiceMock.Setup(x => x.GetMemberId()).Returns(memberId);

        var sut = new CalendarEventController(_outerApiMock.Object, sessionServiceMock.Object, Mock.Of<IValidator<ReviewEventViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.UpdateEvent.UpdateSchoolName, SchoolNameUrl);

        var result = sut.Get(_calendarEventId, new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<ReviewEventViewModel>());
        var vm = actualResult.Model as ReviewEventViewModel;
        vm!.SchoolNameLink.Should().Be(SchoolNameUrl);
    }

    [Test, MoqAutoData]
    public async Task Post_RedirectsToNotifyAttendees(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        [Frozen] Mock<IValidator<ReviewEventViewModel>> validatorMock,
        List<CalendarDetail> calendars,
        GetRegionsResult regionsResult)
    {
        outerAPiMock.Setup(o => o.GetCalendars(It.IsAny<CancellationToken>())).ReturnsAsync(calendars);
        outerAPiMock.Setup(o => o.GetRegions(It.IsAny<CancellationToken>())).ReturnsAsync(regionsResult);

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ReviewEventViewModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        var sessionServiceMock = new Mock<ISessionService>();
        var sessionModel = new EventSessionModel
        {
            CalendarId = calendars.First().Id,
            RegionId = regionsResult.Regions.First().Id
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new CalendarEventController(outerAPiMock.Object, sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.UpdateEvent.NotifyAttendees, NotifyAttendeesUrl);

        var actualResult = await sut.Post(_calendarEventId, new CancellationToken());

        var result = (RedirectToRouteResult)actualResult;
        sut.ModelState.IsValid.Should().BeTrue();
        result.RouteName.Should().Be(RouteNames.UpdateEvent.NotifyAttendees);
    }

    [Test, MoqAutoData]
    public async Task Post_RedirectsToNetworkEventsWhenSessionModelIsNull(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        [Frozen] Mock<IValidator<ReviewEventViewModel>> validatorMock)
    {
        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ReviewEventViewModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        var sessionServiceMock = new Mock<ISessionService>();

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns((EventSessionModel)null!);

        var sut = new CalendarEventController(outerAPiMock.Object, sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);

        var actualResult = await sut.Post(_calendarEventId, new CancellationToken());

        var result = (RedirectToRouteResult)actualResult;
        result.RouteName.Should().Be(RouteNames.NetworkEvents);
    }

    [Test, MoqAutoData]
    public async Task Post_RedirectsToNetworkEvents(
        [Frozen] Mock<IOuterApiClient> outrApiMock,
        [Frozen] Mock<IValidator<ReviewEventViewModel>> validatorMock,
        List<CalendarDetail> calendars,
        GetRegionsResult regionsResult)
    {
        outrApiMock.Setup(o => o.GetCalendars(It.IsAny<CancellationToken>())).ReturnsAsync(calendars);
        outrApiMock.Setup(o => o.GetRegions(It.IsAny<CancellationToken>())).ReturnsAsync(regionsResult);

        var validationResult = new ValidationResult();
        validationResult.Errors.Add(new ValidationFailure("name", "message"));
        validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ReviewEventViewModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        var sessionServiceMock = new Mock<ISessionService>();
        var sessionModel = new EventSessionModel
        {
            CalendarId = calendars.First().Id,
            RegionId = regionsResult.Regions.First().Id,
            EventFormat = EventFormat.InPerson,
            Location = null
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);
        var sut = new CalendarEventController(outrApiMock.Object, sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);

        var actualResult = await sut.Post(_calendarEventId, new CancellationToken());

        var result = (ViewResult)actualResult;
        Assert.That(result.Model, Is.TypeOf<ReviewEventViewModel>());
        (result.Model as ReviewEventViewModel)!.CancelLink.Should().Be(NetworkEventsUrl);

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

        var sut = new CalendarEventController(outerAPiMock.Object, sessionServiceMock.Object, Mock.Of<IValidator<ReviewEventViewModel>>());
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CalendarEvent, CalendarEventUrl);

        var result = sut.GetPreview();
        var actualResult = result.As<ViewResult>();

        using (new AssertionScope())
        {
            actualResult.Model.Should().BeOfType<NetworkEventDetailsViewModel>();
            var vm = actualResult.Model.As<NetworkEventDetailsViewModel>();
            vm.BackLinkUrl.Should().Be(CalendarEventUrl);
            vm.BackLinkDescription.Should().Be(CalendarEventController.PreviewBackLinkDescription);
            vm.PreviewHeader.Should().Be(CalendarEventController.EventPreviewHeader);
            sessionServiceMock.Verify(x => x.Get<EventSessionModel>(), Times.Once);
        }
    }

    [Test]
    public void GetCalendarEventPreview_SessionModelMissing_RedirectsToManageEvents()
    {
        CalendarEventController sut = new(Mock.Of<IOuterApiClient>(), Mock.Of<ISessionService>(), Mock.Of<IValidator<ReviewEventViewModel>>());
        var result = sut.GetPreview();
        result.Should().BeOfType<RedirectToRouteResult>();
        result.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.NetworkEvents);
    }

    [Test, AutoData]
    public async Task GetCalendarEventDetails_ReturnsPreview(string networkEventsUrl, GetCalendarEventQueryResult apiResponse, Guid memberId, Guid calendarEventId, CancellationToken cancellationToken)
    {
        Mock<ISessionService> serviceMock = new();
        serviceMock.Setup(s => s.GetMemberId()).Returns(memberId);
        Mock<IOuterApiClient> apiMock = new();
        apiMock.Setup(a => a.GetCalendarEvent(memberId, calendarEventId, cancellationToken)).ReturnsAsync(apiResponse);
        CalendarEventController sut = new(apiMock.Object, serviceMock.Object, Mock.Of<IValidator<ReviewEventViewModel>>());
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, networkEventsUrl);

        var result = await sut.GetDetails(calendarEventId, cancellationToken);

        using (new AssertionScope())
        {
            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().ViewName.Should().Be(CalendarEventController.PreviewViewPath);
            var vm = result.As<ViewResult>().Model.As<NetworkEventDetailsViewModel>();
            vm.BackLinkUrl.Should().Be(networkEventsUrl);
            vm.BackLinkDescription.Should().Be(CalendarEventController.DetailsBackLinkDescription);
            vm.PreviewHeader.Should().Be(CalendarEventController.EventDetailsHeader);
        }
    }
}