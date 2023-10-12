using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Admin.Aan.Application.OuterApi.Regions;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Controllers.ManageEvent;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;
using SFA.DAS.Admin.Aan.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;
using Calendar = SFA.DAS.Admin.Aan.Application.OuterApi.Calendar.Calendar;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers.ManageEvent;

public class CheckYourAnswersControllerTests
{
    private static readonly string NetworkEventsUrl = Guid.NewGuid().ToString();
    private static readonly string PostUrl = Guid.NewGuid().ToString();
    private static readonly string EventFormatUrl = Guid.NewGuid().ToString();
    private static readonly string EventLocationUrl = Guid.NewGuid().ToString();
    private static readonly string EventTypeUrl = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void GetCheckYourAnswers_ReturnsApiResponse(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        [Frozen] Mock<IValidator<CheckYourAnswersViewModel>> validatorMock,
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

        var sut = new CheckYourAnswersController(sessionServiceMock.Object, outerAPiMock.Object, validatorMock.Object);

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
    public void GetCheckYourAnswers_HasSeenPreviewFalse_SettingToTrue(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        [Frozen] Mock<IValidator<CheckYourAnswersViewModel>> validatorMock,
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
            HasSeenPreview = false
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new CheckYourAnswersController(sessionServiceMock.Object, outerAPiMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);
        var result = sut.Get(new CancellationToken());

        sessionServiceMock.Verify(x => x.Set(It.Is<EventSessionModel>(s => s.HasSeenPreview == true)), Times.Once());
    }

    [Test, MoqAutoData]
    public void GetCheckYourAnswers_HasSeenPreviewTrue_NoUpdateToSessionModel(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        [Frozen] Mock<IValidator<CheckYourAnswersViewModel>> validatorMock,
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
            HasSeenPreview = true
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new CheckYourAnswersController(sessionServiceMock.Object, outerAPiMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);
        var result = sut.Get(new CancellationToken());

        sessionServiceMock.Verify(x => x.Set(It.IsAny<EventSessionModel>()), Times.Never());
    }

    [Test, MoqAutoData]
    public void Post_ReturnsExpectedPostLink(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        [Frozen] Mock<IValidator<CheckYourAnswersViewModel>> validatorMock,
        List<Calendar> calendars,
        GetRegionsResult regionsResult)
    {

        outerAPiMock.Setup(o => o.GetCalendars(It.IsAny<CancellationToken>())).ReturnsAsync(calendars);
        outerAPiMock.Setup(o => o.GetRegions(It.IsAny<CancellationToken>())).ReturnsAsync(regionsResult);

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.ValidateAsync(It.IsAny<CheckYourAnswersViewModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        var sessionServiceMock = new Mock<ISessionService>();
        var sessionModel = new EventSessionModel
        {
            CalendarId = calendars.First().Id,
            RegionId = regionsResult.Regions.First().Id,
            DateOfEvent = DateTime.Today,
            StartHour = DateTime.Today.Hour,
            StartMinutes = DateTime.Today.Minute,
            EndHour = DateTime.Today.Hour,
            EndMinutes = DateTime.Today.Minute,
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new CheckYourAnswersController(sessionServiceMock.Object, outerAPiMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.ManageEvent.CheckYourAnswers, PostUrl);
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);

        var response = sut.Post(new CancellationToken());

        var result = response.Result as RedirectToRouteResult;
        sut.ModelState.IsValid.Should().BeTrue();
        result!.RouteName.Should().Be(RouteNames.ManageEvent.EventPublished);
        sessionServiceMock.Verify(s => s.Delete(nameof(EventSessionModel)));
    }

    [Test, MoqAutoData]
    public void GetCheckYourAnswers_ReturnsExpectedEventFormatLink(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        [Frozen] Mock<IValidator<CheckYourAnswersViewModel>> validatorMock,
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

        var sut = new CheckYourAnswersController(sessionServiceMock.Object, outerAPiMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.ManageEvent.EventFormat, EventFormatUrl);

        var result = sut.Get(new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<CheckYourAnswersViewModel>());
        var vm = actualResult.Model as CheckYourAnswersViewModel;
        vm!.EventFormatLink.Should().Be(EventFormatUrl);
    }

    [Test, MoqAutoData]
    public void GetCheckYourAnswers_ReturnsExpectedEventLocationLink(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        [Frozen] Mock<IValidator<CheckYourAnswersViewModel>> validatorMock,
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

        var sut = new CheckYourAnswersController(sessionServiceMock.Object, outerAPiMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.ManageEvent.Location, EventLocationUrl);

        var result = sut.Get(new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<CheckYourAnswersViewModel>());
        var vm = actualResult.Model as CheckYourAnswersViewModel;
        vm!.EventLocationLink.Should().Be(EventLocationUrl);
    }

    [Test, MoqAutoData]
    public void GetCheckYourAnswers_ReturnsExpectedEventTypeLink(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        [Frozen] Mock<IValidator<CheckYourAnswersViewModel>> validatorMock,
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

        var sut = new CheckYourAnswersController(sessionServiceMock.Object, outerAPiMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.ManageEvent.EventType, EventTypeUrl);

        var result = sut.Get(new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<CheckYourAnswersViewModel>());
        var vm = actualResult.Model as CheckYourAnswersViewModel;
        vm!.EventTypeLink.Should().Be(EventTypeUrl);
    }

    [Test, MoqAutoData]
    public async Task Post_ValidationErrors(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        [Frozen] Mock<IValidator<CheckYourAnswersViewModel>> validatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        List<Calendar> calendars,
        GetRegionsResult regionsResult)
    {
        outerAPiMock.Setup(o => o.GetCalendars(It.IsAny<CancellationToken>())).ReturnsAsync(calendars);
        outerAPiMock.Setup(o => o.GetRegions(It.IsAny<CancellationToken>())).ReturnsAsync(regionsResult);

        var sessionModel = new EventSessionModel
        {
            CalendarId = calendars.First()!.Id,
            RegionId = regionsResult.Regions!.First().Id
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new CheckYourAnswersController(sessionServiceMock.Object, outerAPiMock.Object, validatorMock.Object);
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);

        sut.ModelState.AddModelError("key", "message");

        var submitModel = new CheckYourAnswersViewModel { CancelLink = NetworkEventsUrl };

        var actualResult = await sut.Post(new CancellationToken());

        var result = (ViewResult)actualResult;

        sut.ModelState.IsValid.Should().BeFalse();
        Assert.That(result.Model, Is.TypeOf<CheckYourAnswersViewModel>());
        (result.Model as CheckYourAnswersViewModel)!.CancelLink.Should().Be(NetworkEventsUrl);
        sessionServiceMock.Verify(s => s.Set(It.IsAny<EventSessionModel>()), Times.Never());
    }
}