﻿using AutoFixture.NUnit3;
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

[TestFixture]
public class CheckYourAnswersControllerTests
{
    private static readonly string NetworkEventsUrl = Guid.NewGuid().ToString();
    private static readonly string PostUrl = Guid.NewGuid().ToString();
    private static readonly string EventFormatUrl = Guid.NewGuid().ToString();
    private static readonly string EventLocationUrl = Guid.NewGuid().ToString();
    private static readonly string EventTypeUrl = Guid.NewGuid().ToString();
    private static readonly string EventDateAndTimeUrl = Guid.NewGuid().ToString();
    private static readonly string EventDescriptionUrl = Guid.NewGuid().ToString();
    private static readonly string EventHasGuestSpeakersUrl = Guid.NewGuid().ToString();
    private static readonly string OrganiserDetailsUrl = Guid.NewGuid().ToString();
    private static readonly string IsAtSchoolUrl = Guid.NewGuid().ToString();
    private static readonly string SchoolNameUrl = Guid.NewGuid().ToString();
    private static readonly string PreviewUrl = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void GetCheckYourAnswers_ReturnsApiResponse(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        List<CalendarDetail> calendars,
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

        Assert.That(actualResult!.Model, Is.TypeOf<CheckAnswersViewModel>());
        var vm = actualResult.Model as CheckAnswersViewModel;
        vm!.CancelLink.Should().Be(NetworkEventsUrl);
        vm.PageTitle.Should().Be(Application.Constants.CreateEvent.PageTitle);
        vm.EventRegion.Should().Be(regionsResult.Regions.First(x => x.Id == sessionModel.RegionId).Area);
        vm.EventType.Should().Be(calendars.First(x => x.Id == sessionModel.CalendarId).CalendarName);
    }

    [Test, MoqAutoData]
    public void GetCheckYourAnswers_HasSeenPreviewFalse_SettingToTrue(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        List<CalendarDetail> calendars,
        GetRegionsResult regionsResult)
    {
        outerAPiMock.Setup(o => o.GetCalendars(It.IsAny<CancellationToken>())).ReturnsAsync(calendars);
        outerAPiMock.Setup(o => o.GetRegions(It.IsAny<CancellationToken>())).ReturnsAsync(regionsResult);

        var sessionServiceMock = new Mock<ISessionService>();
        var sessionModel = new EventSessionModel
        {
            CalendarId = calendars.First().Id,
            RegionId = regionsResult.Regions.First().Id,
            HasSeenPreview = false
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new CheckYourAnswersController(sessionServiceMock.Object, outerAPiMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);
        var result = sut.Get(new CancellationToken());

        sessionServiceMock.Verify(x => x.Set(It.Is<EventSessionModel>(s => s.HasSeenPreview == true)), Times.Once());
    }

    [TestCase(true, true, false)]
    [TestCase(true, false, true)]
    [TestCase(false, true, true)]
    [TestCase(false, false, true)]
    public void GetCheckYourAnswers_SeenPreviewAndDirectCallFromCheckYourAnswersSettings_CheckUpdateToSessionModel(bool hasSeenPreview, bool directCallFromCheckYourAnswers, bool callsToSetEventSessionModel)
    {
        var outerAPiMock = new Mock<IOuterApiClient>();
        var calendars = new List<CalendarDetail>
        {
            new() {CalendarName = "cal 1", Id = 1}
        };

        var regionsResult = new GetRegionsResult
        {
            Regions = [new(1, "London", 1)]
        };

        outerAPiMock.Setup(o => o.GetCalendars(It.IsAny<CancellationToken>())).ReturnsAsync(calendars);
        outerAPiMock.Setup(o => o.GetRegions(It.IsAny<CancellationToken>())).ReturnsAsync(regionsResult);

        var sessionServiceMock = new Mock<ISessionService>();
        var sessionModel = new EventSessionModel
        {
            CalendarId = calendars.First().Id,
            RegionId = regionsResult.Regions.First().Id,
            HasSeenPreview = hasSeenPreview,
            IsDirectCallFromCheckYourAnswers = directCallFromCheckYourAnswers
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new CheckYourAnswersController(sessionServiceMock.Object, outerAPiMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);
        var result = sut.Get(new CancellationToken());
        if (callsToSetEventSessionModel)
        {
            sessionServiceMock.Verify(x => x.Set(It.IsAny<EventSessionModel>()), Times.Once());
        }
        else
        {
            sessionServiceMock.Verify(x => x.Set(It.IsAny<EventSessionModel>()), Times.Never());
        }
    }

    [Test, MoqAutoData]
    public void Post_ReturnsExpectedPostLink(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        List<CalendarDetail> calendars,
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
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.CheckYourAnswers, PostUrl);
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);

        var response = sut.Post(new CancellationToken());

        var result = response.Result as RedirectToRouteResult;
        sut.ModelState.IsValid.Should().BeTrue();
        result!.RouteName.Should().Be(RouteNames.CreateEvent.EventPublished);
        sessionServiceMock.Verify(s => s.Delete(nameof(EventSessionModel)));
    }

    [Test, MoqAutoData]
    public void GetCheckYourAnswers_ReturnsExpectedEventFormatLink(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        List<CalendarDetail> calendars,
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

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.EventFormat, EventFormatUrl);

        var result = sut.Get(new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<CheckAnswersViewModel>());
        var vm = actualResult.Model as CheckAnswersViewModel;
        vm!.EventFormatLink.Should().Be(EventFormatUrl);
    }

    [Test, MoqAutoData]
    public void GetCheckYourAnswers_ReturnsExpectedEventLocationLink(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        List<CalendarDetail> calendars,
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

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.Location, EventLocationUrl);

        var result = sut.Get(new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<CheckAnswersViewModel>());
        var vm = actualResult.Model as CheckAnswersViewModel;
        vm!.EventLocationLink.Should().Be(EventLocationUrl);
    }

    [Test, MoqAutoData]
    public void GetCheckYourAnswers_ReturnsExpectedEventTypeLink(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        List<CalendarDetail> calendars,
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

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.EventType, EventTypeUrl);

        var result = sut.Get(new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<CheckAnswersViewModel>());
        var vm = actualResult.Model as CheckAnswersViewModel;
        vm!.EventTypeLink.Should().Be(EventTypeUrl);
    }

    [Test, MoqAutoData]
    public void GetCheckYourAnswers_ReturnsExpectedEventDateAndTimeLink(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        
        List<CalendarDetail> calendars,
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

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.DateAndTime, EventDateAndTimeUrl);

        var result = sut.Get(new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<CheckAnswersViewModel>());
        var vm = actualResult.Model as CheckAnswersViewModel;
        vm!.EventDateTimeLink.Should().Be(EventDateAndTimeUrl);
    }

    [Test, MoqAutoData]
    public void GetCheckYourAnswers_ReturnsExpectedEventDescriptionLink(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        List<CalendarDetail> calendars,
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

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.Description, EventDescriptionUrl);

        var result = sut.Get(new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<CheckAnswersViewModel>());
        var vm = actualResult.Model as CheckAnswersViewModel;
        vm!.EventDescriptionLink.Should().Be(EventDescriptionUrl);
    }

    [Test, MoqAutoData]
    public void GetCheckYourAnswers_ReturnsExpectedPreviewLink(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        List<CalendarDetail> calendars,
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

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.PreviewEvent, PreviewUrl);

        var result = sut.Get(new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<CheckAnswersViewModel>());
        var vm = actualResult.Model as CheckAnswersViewModel;
        vm!.PreviewLink.Should().Be(PreviewUrl);
    }

    [Test, MoqAutoData]
    public void GetCheckYourAnswers_ReturnsExpectedHasGuestSpeakersLink(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        List<CalendarDetail> calendars,
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

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.HasGuestSpeakers, EventHasGuestSpeakersUrl);

        var result = sut.Get(new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<CheckAnswersViewModel>());
        var vm = actualResult.Model as CheckAnswersViewModel;
        vm!.HasGuestSpeakersLink.Should().Be(EventHasGuestSpeakersUrl);
    }

    [Test, MoqAutoData]
    public void GetCheckYourAnswers_ReturnsExpectedOrganiserDetailsLink(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        List<CalendarDetail> calendars,
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

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.OrganiserDetails, OrganiserDetailsUrl);

        var result = sut.Get(new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<CheckAnswersViewModel>());
        var vm = actualResult.Model as CheckAnswersViewModel;
        vm!.OrganiserDetailsLink.Should().Be(OrganiserDetailsUrl);
    }

    [Test, MoqAutoData]
    public void GetCheckYourAnswers_ReturnsExpectedIsAtSchoolLink(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        List<CalendarDetail> calendars,
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

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.IsAtSchool, IsAtSchoolUrl);

        var result = sut.Get(new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<CheckAnswersViewModel>());
        var vm = actualResult.Model as CheckAnswersViewModel;
        vm!.IsAtSchoolLink.Should().Be(IsAtSchoolUrl);
    }


    [Test, MoqAutoData]
    public void GetCheckYourAnswers_ReturnsExpectedSchoolNameLink(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        List<CalendarDetail> calendars,
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

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.SchoolName, SchoolNameUrl);

        var result = sut.Get(new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<CheckAnswersViewModel>());
        var vm = actualResult.Model as CheckAnswersViewModel;
        vm!.SchoolNameLink.Should().Be(SchoolNameUrl);
    }
}