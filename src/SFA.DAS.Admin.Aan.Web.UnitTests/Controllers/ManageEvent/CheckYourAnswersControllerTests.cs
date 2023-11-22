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
        [Frozen] Mock<IValidator<ReviewEventViewModel>> validatorMock,
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

        Assert.That(actualResult!.Model, Is.TypeOf<ReviewEventViewModel>());
        var vm = actualResult.Model as ReviewEventViewModel;
        vm!.CancelLink.Should().Be(NetworkEventsUrl);
        vm.PageTitle.Should().Be(Application.Constants.CreateEvent.PageTitle);
        vm.EventRegion.Should().Be(regionsResult.Regions.First(x => x.Id == sessionModel.RegionId).Area);
        vm.EventType.Should().Be(calendars.First(x => x.Id == sessionModel.CalendarId).CalendarName);
    }

    [Test, MoqAutoData]
    public void GetCheckYourAnswers_HasSeenPreviewFalse_SettingToTrue(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        [Frozen] Mock<IValidator<ReviewEventViewModel>> validatorMock,
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

    [TestCase(true, true, false)]
    [TestCase(true, false, true)]
    [TestCase(false, true, true)]
    [TestCase(false, false, true)]
    public void GetCheckYourAnswers_SeenPreviewAndDirectCallFromCheckYourAnswersSettings_CheckUpdateToSessionModel(bool hasSeenPreview, bool directCallFromCheckYourAnswers, bool callsToSetEventSessionModel)
    {
        var outerAPiMock = new Mock<IOuterApiClient>();
        var validatorMock = new Mock<IValidator<ReviewEventViewModel>>();
        var calendars = new List<Calendar>
        {
            new Calendar {CalendarName = "cal 1", Id = 1}
        };

        var regionsResult = new GetRegionsResult
        {
            Regions = new List<Region>
            {
                new Region(1, "London", 1)
            }
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

        var sut = new CheckYourAnswersController(sessionServiceMock.Object, outerAPiMock.Object, validatorMock.Object);

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
        [Frozen] Mock<IValidator<ReviewEventViewModel>> validatorMock,
        List<Calendar> calendars,
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
        [Frozen] Mock<IValidator<ReviewEventViewModel>> validatorMock,
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

        Assert.That(actualResult!.Model, Is.TypeOf<ReviewEventViewModel>());
        var vm = actualResult.Model as ReviewEventViewModel;
        vm!.EventFormatLink.Should().Be(EventFormatUrl);
    }

    [Test, MoqAutoData]
    public void GetCheckYourAnswers_ReturnsExpectedEventLocationLink(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        [Frozen] Mock<IValidator<ReviewEventViewModel>> validatorMock,
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

        Assert.That(actualResult!.Model, Is.TypeOf<ReviewEventViewModel>());
        var vm = actualResult.Model as ReviewEventViewModel;
        vm!.EventLocationLink.Should().Be(EventLocationUrl);
    }

    [Test, MoqAutoData]
    public void GetCheckYourAnswers_ReturnsExpectedEventTypeLink(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        [Frozen] Mock<IValidator<ReviewEventViewModel>> validatorMock,
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

        Assert.That(actualResult!.Model, Is.TypeOf<ReviewEventViewModel>());
        var vm = actualResult.Model as ReviewEventViewModel;
        vm!.EventTypeLink.Should().Be(EventTypeUrl);
    }

    [Test, MoqAutoData]
    public void GetCheckYourAnswers_ReturnsExpectedEventDateAndTimeLink(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        [Frozen] Mock<IValidator<ReviewEventViewModel>> validatorMock,
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

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.ManageEvent.DateAndTime, EventDateAndTimeUrl);

        var result = sut.Get(new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<ReviewEventViewModel>());
        var vm = actualResult.Model as ReviewEventViewModel;
        vm!.EventDateTimeLink.Should().Be(EventDateAndTimeUrl);
    }

    [Test, MoqAutoData]
    public void GetCheckYourAnswers_ReturnsExpectedEventDescriptionLink(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        [Frozen] Mock<IValidator<ReviewEventViewModel>> validatorMock,
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

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.ManageEvent.Description, EventDescriptionUrl);

        var result = sut.Get(new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<ReviewEventViewModel>());
        var vm = actualResult.Model as ReviewEventViewModel;
        vm!.EventDescriptionLink.Should().Be(EventDescriptionUrl);
    }

    [Test, MoqAutoData]
    public void GetCheckYourAnswers_ReturnsExpectedPreviewLink(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        [Frozen] Mock<IValidator<ReviewEventViewModel>> validatorMock,
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

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.ManageEvent.PreviewEvent, PreviewUrl);

        var result = sut.Get(new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<ReviewEventViewModel>());
        var vm = actualResult.Model as ReviewEventViewModel;
        vm!.PreviewLink.Should().Be(PreviewUrl);
    }

    [Test, MoqAutoData]
    public void GetCheckYourAnswers_ReturnsExpectedHasGuestSpeakersLink(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        [Frozen] Mock<IValidator<ReviewEventViewModel>> validatorMock,
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

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.ManageEvent.HasGuestSpeakers, EventHasGuestSpeakersUrl);

        var result = sut.Get(new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<ReviewEventViewModel>());
        var vm = actualResult.Model as ReviewEventViewModel;
        vm!.HasGuestSpeakersLink.Should().Be(EventHasGuestSpeakersUrl);
    }

    [Test, MoqAutoData]
    public void GetCheckYourAnswers_ReturnsExpectedOrganiserDetailsLink(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        [Frozen] Mock<IValidator<ReviewEventViewModel>> validatorMock,
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

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.ManageEvent.OrganiserDetails, OrganiserDetailsUrl);

        var result = sut.Get(new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<ReviewEventViewModel>());
        var vm = actualResult.Model as ReviewEventViewModel;
        vm!.OrganiserDetailsLink.Should().Be(OrganiserDetailsUrl);
    }

    [Test, MoqAutoData]
    public void GetCheckYourAnswers_ReturnsExpectedIsAtSchoolLink(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        [Frozen] Mock<IValidator<ReviewEventViewModel>> validatorMock,
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

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.ManageEvent.IsAtSchool, IsAtSchoolUrl);

        var result = sut.Get(new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<ReviewEventViewModel>());
        var vm = actualResult.Model as ReviewEventViewModel;
        vm!.IsAtSchoolLink.Should().Be(IsAtSchoolUrl);
    }


    [Test, MoqAutoData]
    public void GetCheckYourAnswers_ReturnsExpectedSchoolNameLink(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        [Frozen] Mock<IValidator<ReviewEventViewModel>> validatorMock,
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

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.ManageEvent.SchoolName, SchoolNameUrl);

        var result = sut.Get(new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<ReviewEventViewModel>());
        var vm = actualResult.Model as ReviewEventViewModel;
        vm!.SchoolNameLink.Should().Be(SchoolNameUrl);
    }

    [Test, MoqAutoData]
    public async Task Post_ValidationErrors(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        [Frozen] Mock<IValidator<ReviewEventViewModel>> validatorMock,
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

        var submitModel = new ReviewEventViewModel { CancelLink = NetworkEventsUrl };

        var actualResult = await sut.Post(new CancellationToken());

        var result = (ViewResult)actualResult;

        sut.ModelState.IsValid.Should().BeFalse();
        Assert.That(result.Model, Is.TypeOf<ReviewEventViewModel>());
        (result.Model as ReviewEventViewModel)!.CancelLink.Should().Be(NetworkEventsUrl);
        sessionServiceMock.Verify(s => s.Set(It.IsAny<EventSessionModel>()), Times.Never());
    }
}