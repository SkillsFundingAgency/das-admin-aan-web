using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Aan.SharedUi.Constants;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Controllers.ManageEvent;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;
using SFA.DAS.Admin.Aan.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers.ManageEvent;
public class EventDateAndTimeControllerTests
{
    private static readonly string NetworkEventsUrl = Guid.NewGuid().ToString();
    private static readonly string PostUrl = Guid.NewGuid().ToString();
    private static readonly string CheckYourAnswersUrl = Guid.NewGuid().ToString();
    private static readonly string CalendarEventUrl = Guid.NewGuid().ToString();
    private static readonly string UpdateEventDateAndTimeUrl = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void Get_ReturnsCreateEventDateTimeViewModel(
        [Greedy] EventDateAndTimeController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);
        var result = (ViewResult)sut.Get();

        Assert.That(result.Model, Is.TypeOf<EventDateAndTimeViewModel>());
        var vm = result.Model as EventDateAndTimeViewModel;
        vm!.CancelLink.Should().Be(NetworkEventsUrl);
        vm.PageTitle.Should().Be(Application.Constants.CreateEvent.PageTitle);
    }

    [Test, MoqAutoData]
    public void Get_ReturnsExpectedPostLink(
        [Greedy] EventDateAndTimeController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.DateAndTime, PostUrl);
        var result = (ViewResult)sut.Get();
        Assert.That(result.Model, Is.TypeOf<EventDateAndTimeViewModel>());
        var vm = result.Model as EventDateAndTimeViewModel;
        vm!.PostLink.Should().Be(PostUrl);
    }

    [Test, MoqAutoData]
    public void Get_HasSeenPreviewTrue_CancelLinkIsCheckYourAnswers()
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<EventDateAndTimeViewModel>>();

        var sessionModel = new EventSessionModel
        {
            EventTitle = "title",
            CalendarId = 1,
            RegionId = 2,
            EventFormat = EventFormat.Hybrid,
            HasSeenPreview = true
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new EventDateAndTimeController(sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.CheckYourAnswers, CheckYourAnswersUrl);
        var actualResult = sut.Get();
        var result = actualResult.As<ViewResult>();

        Assert.That(result.Model, Is.TypeOf<EventDateAndTimeViewModel>());
        var vm = result.Model as EventDateAndTimeViewModel;
        vm!.CancelLink.Should().Be(CheckYourAnswersUrl);
    }

    [Test, MoqAutoData]
    public void Get_IsAlreadyPublishedTrue_CancelLinkIsCalendarEvent()
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<EventDateAndTimeViewModel>>();

        var sessionModel = new EventSessionModel { IsAlreadyPublished = true };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new EventDateAndTimeController(sessionServiceMock.Object, validatorMock.Object);


        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CalendarEvent, CalendarEventUrl);
        var result = (ViewResult)sut.Get();

        Assert.That(result.Model, Is.TypeOf<EventDateAndTimeViewModel>());
        var vm = result.Model as EventDateAndTimeViewModel;
        vm!.CancelLink.Should().Be(CalendarEventUrl);
    }

    [Test, MoqAutoData]
    public void Post_HasSeenPreview_False_RedirectsToLocation()
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<EventDateAndTimeViewModel>>();

        var sessionModel = new EventSessionModel
        {
            IsAlreadyPublished = false,
            HasSeenPreview = false
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var submitModel = new EventDateAndTimeViewModel
        {
            DateOfEvent = DateTime.Today.AddDays(1),
            StartHour = 12,
            StartMinutes = 0,
            EndHour = 13,
            EndMinutes = 30
        };

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);

        var sut = new EventDateAndTimeController(sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.CheckYourAnswers, CheckYourAnswersUrl);
        var actualResult = sut.Post(submitModel);
        var result = actualResult.As<RedirectToRouteResult>();

        result.RouteName.Should().Be(RouteNames.CreateEvent.Location);
    }

    [Test, MoqAutoData]
    public void Post_HasSeenPreview_True_RedirectsToCheckYourAnswers()
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<EventDateAndTimeViewModel>>();

        var sessionModel = new EventSessionModel
        {
            HasSeenPreview = true
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var submitModel = new EventDateAndTimeViewModel
        {
            DateOfEvent = DateTime.Today.AddDays(1),
            StartHour = 12,
            StartMinutes = 0,
            EndHour = 13,
            EndMinutes = 30
        };

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);

        var sut = new EventDateAndTimeController(sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.CheckYourAnswers, CheckYourAnswersUrl);
        var actualResult = sut.Post(submitModel);
        var result = actualResult.As<RedirectToRouteResult>();

        result.RouteName.Should().Be(RouteNames.CreateEvent.CheckYourAnswers);
    }

    [Test, MoqAutoData]
    public void Get_IsAlreadyPublishedTrue_PostLinkLinkIsUpdateCalendarEvent()
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<EventDateAndTimeViewModel>>();

        var sessionModel = new EventSessionModel { IsAlreadyPublished = true };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new EventDateAndTimeController(sessionServiceMock.Object, validatorMock.Object);


        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.UpdateEvent.UpdateDateAndTime, UpdateEventDateAndTimeUrl);
        var result = (ViewResult)sut.Get();

        Assert.That(result.Model, Is.TypeOf<EventDateAndTimeViewModel>());
        var vm = result.Model as EventDateAndTimeViewModel;
        vm!.PostLink.Should().Be(UpdateEventDateAndTimeUrl);
    }

    [Ignore("Fix later")]
    [Test, MoqAutoData]
    public void Post_IsAlreadyPublishedTrue_RedirectsToCalendarEvent()
    {
        var calendarEventId = Guid.NewGuid();
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<EventDateAndTimeViewModel>>();

        var sessionModel = new EventSessionModel
        {
            CalendarEventId = calendarEventId,
            IsAlreadyPublished = true,
            DateOfEvent = DateTime.Today.AddDays(1),
            StartHour = 12,
            StartMinutes = 0,
            EndHour = 13,
            EndMinutes = 30
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var submitModel = new EventDateAndTimeViewModel();

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);

        var sut = new EventDateAndTimeController(sessionServiceMock.Object, validatorMock.Object);

        var result = (RedirectToRouteResult)sut.Post(submitModel);
        result.RouteName.Should().Be(RouteNames.CalendarEvent);
    }

    [Ignore("Fix later")]
    [Test, MoqAutoData]
    public void Post_IsAlreadyPublishedTrue_SetsHasChangedEventToTrue()
    {
        var calendarEventId = Guid.NewGuid();
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<EventDateAndTimeViewModel>>();

        var sessionModel = new EventSessionModel
        {
            CalendarEventId = calendarEventId,
            IsAlreadyPublished = true,
            DateOfEvent = DateTime.Today.AddDays(1),
            StartHour = 12,
            StartMinutes = 0,
            EndHour = 13,
            EndMinutes = 30,
            HasChangedEvent = false
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var submitModel = new EventDateAndTimeViewModel();

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);

        var sut = new EventDateAndTimeController(sessionServiceMock.Object, validatorMock.Object);

        sut.Post(submitModel);
        sessionServiceMock.Verify(s => s.Set(It.Is<EventSessionModel>(m => m.HasChangedEvent == true)), Times.Once);
    }

    [TestCase(12, 0, 13, 0)]
    public void Post_SetEventDateTimeOnSessionModel(int startHour, int startMinutes, int endHour, int endMinutes)
    {
        var dateOfEvent = DateTime.Today.AddDays(1);

        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<EventDateAndTimeViewModel>>();

        var sessionModel = new EventSessionModel();

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var submitModel = new EventDateAndTimeViewModel
        {
            DateOfEvent = dateOfEvent,
            StartHour = startHour,
            StartMinutes = startMinutes,
            EndHour = endHour,
            EndMinutes = endMinutes
        };

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);

        var sut = new EventDateAndTimeController(sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);

        var result = (RedirectToRouteResult)sut.Post(submitModel);

        sut.ModelState.IsValid.Should().BeTrue();
        sessionServiceMock.Verify(s => s.Set(It.Is<EventSessionModel>(m
            => m.DateOfEvent == dateOfEvent &&
               m.StartHour == startHour &&
               m.StartMinutes == startMinutes &&
               m.EndHour == endHour &&
               m.EndMinutes == endMinutes)));
        result.RouteName.Should().Be(RouteNames.CreateEvent.Location);
    }

    [Test, MoqAutoData]
    public void Post_WhenNoSelectionOfEventDateTime_Errors(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] EventDateAndTimeController sut)
    {
        sut.ModelState.AddModelError("key", "message");

        var submitModel = new EventDateAndTimeViewModel { CancelLink = NetworkEventsUrl };

        var result = (ViewResult)sut.Post(submitModel);

        sut.ModelState.IsValid.Should().BeFalse();
        Assert.That(result.Model, Is.TypeOf<EventDateAndTimeViewModel>());
        (result.Model as EventDateAndTimeViewModel)!.CancelLink.Should().Be(NetworkEventsUrl);
        sessionServiceMock.Verify(s => s.Set(It.IsAny<EventSessionModel>()), Times.Never());
    }
}
