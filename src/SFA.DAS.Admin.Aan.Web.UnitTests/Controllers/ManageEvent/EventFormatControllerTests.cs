using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Aan.SharedUi.Constants;
using SFA.DAS.Admin.Aan.Application.Constants;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Controllers.ManageEvent;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;
using SFA.DAS.Admin.Aan.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers.ManageEvent;
public class EventFormatControllerTests
{
    private static readonly string NetworkEventsUrl = Guid.NewGuid().ToString();
    private static readonly string CheckYourAnswersUrl = Guid.NewGuid().ToString();
    private static readonly string PostUrl = Guid.NewGuid().ToString();
    private static readonly string CalendarEventUrl = Guid.NewGuid().ToString();
    private static readonly string UpdateEventFormatUrl = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void Get_ReturnsCreateEventFormatViewModel(
        [Greedy] EventFormatController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);
        var result = (ViewResult)sut.Get();

        Assert.That(result.Model, Is.TypeOf<EventFormatViewModel>());
        var vm = result.Model as EventFormatViewModel;
        vm!.CancelLink.Should().Be(NetworkEventsUrl);
        vm.PageTitle.Should().Be(CreateEvent.PageTitle);
    }

    [Test]
    public void Get_HasSeenPreviewTrue_CancelLinkIsCheckYourAnswers()
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<EventFormatViewModel>>();

        var sessionModel = new EventSessionModel { HasSeenPreview = true, IsAlreadyPublished = false };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new EventFormatController(sessionServiceMock.Object, validatorMock.Object);


        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.CheckYourAnswers, CheckYourAnswersUrl);
        var result = (ViewResult)sut.Get();

        Assert.That(result.Model, Is.TypeOf<EventFormatViewModel>());
        var vm = result.Model as EventFormatViewModel;
        vm!.CancelLink.Should().Be(CheckYourAnswersUrl);
    }

    [Test]
    public void Get_IsAlreadyPublishedTrue_CancelLinkIsCalendarEvent()
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<EventFormatViewModel>>();

        var sessionModel = new EventSessionModel { IsAlreadyPublished = true };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new EventFormatController(sessionServiceMock.Object, validatorMock.Object);


        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CalendarEvent, CalendarEventUrl);
        var result = (ViewResult)sut.Get();

        Assert.That(result.Model, Is.TypeOf<EventFormatViewModel>());
        var vm = result.Model as EventFormatViewModel;
        vm!.CancelLink.Should().Be(CalendarEventUrl);
    }

    [Test, MoqAutoData]
    public void Get_ReturnsExpectedPostLink(
        [Greedy] EventFormatController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.EventFormat, PostUrl);
        var result = (ViewResult)sut.Get();
        Assert.That(result.Model, Is.TypeOf<EventFormatViewModel>());
        var vm = result.Model as EventFormatViewModel;
        vm!.PostLink.Should().Be(PostUrl);
    }

    [Test]
    public void Get_IsAlreadyPublishedTrue_PostLinkLinkIsUpdateCalendarEvent()
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<EventFormatViewModel>>();

        var sessionModel = new EventSessionModel { IsAlreadyPublished = true };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new EventFormatController(sessionServiceMock.Object, validatorMock.Object);


        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.UpdateEvent.UpdateEventFormat, UpdateEventFormatUrl);
        var result = (ViewResult)sut.Get();

        Assert.That(result.Model, Is.TypeOf<EventFormatViewModel>());
        var vm = result.Model as EventFormatViewModel;
        vm!.PostLink.Should().Be(UpdateEventFormatUrl);
    }

    [TestCase(EventFormat.Hybrid, true, UpdateEvent.PageTitle)]
    [TestCase(EventFormat.Hybrid, false, CreateEvent.PageTitle)]
    [TestCase(EventFormat.InPerson, true, UpdateEvent.PageTitle)]
    [TestCase(EventFormat.InPerson, false, CreateEvent.PageTitle)]
    [TestCase(EventFormat.Online, true, UpdateEvent.PageTitle)]
    [TestCase(EventFormat.Online, false, CreateEvent.PageTitle)]
    public void Get_ReturnsExpectedPageTitle(EventFormat eventFormat, bool isAlreadyPublished, string pageTitle)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var sessionModel = new EventSessionModel { IsAlreadyPublished = isAlreadyPublished, EventFormat = eventFormat, CalendarEventId = Guid.NewGuid() };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new EventFormatController(sessionServiceMock.Object, Mock.Of<IValidator<EventFormatViewModel>>());
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);

        var result = (ViewResult)sut.Get();

        Assert.That(result.Model, Is.TypeOf<EventFormatViewModel>());
        var vm = result.Model as EventFormatViewModel;
        vm!.PageTitle.Should().Be(pageTitle);
    }

    [TestCase(EventFormat.InPerson)]
    [TestCase(EventFormat.Hybrid)]
    [TestCase(EventFormat.Online)]
    [TestCase(null)]
    public void Post_SetEventFormatOnSessionModel(EventFormat? eventFormat)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<EventFormatViewModel>>();

        var sessionModel = new EventSessionModel();

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var submitModel = new EventFormatViewModel { EventFormat = eventFormat };

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);

        var sut = new EventFormatController(sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);

        var result = (RedirectToRouteResult)sut.Post(submitModel);

        sut.ModelState.IsValid.Should().BeTrue();
        sessionServiceMock.Verify(s => s.Set(It.Is<EventSessionModel>(m => m.EventFormat == eventFormat)));
        result.RouteName.Should().Be(RouteNames.CreateEvent.EventType);
    }

    [Test]
    public void Post_IsAlreadyPublishedTrue_RedirectsToUpdateLocation()
    {
        var calendarEventId = Guid.NewGuid();
        var eventFormat = EventFormat.Hybrid;
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<EventFormatViewModel>>();

        var sessionModel = new EventSessionModel
        {
            CalendarEventId = calendarEventId,
            IsAlreadyPublished = true
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var submitModel = new EventFormatViewModel { EventFormat = eventFormat };

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);

        var sut = new EventFormatController(sessionServiceMock.Object, validatorMock.Object);

        var result = (RedirectToRouteResult)sut.Post(submitModel);
        result.RouteName.Should().Be(RouteNames.UpdateEvent.UpdateLocation);
        result.RouteValues!["CalendarEventId"].Should().Be(calendarEventId);
    }

    [Test]
    public void Post_IsAlreadyPublishedTrue_SetsHasChangedEventToTrue()
    {
        var calendarEventId = Guid.NewGuid();
        var eventFormat = EventFormat.Hybrid;
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<EventFormatViewModel>>();

        var sessionModel = new EventSessionModel
        {
            CalendarEventId = calendarEventId,
            IsAlreadyPublished = true,
            HasChangedEvent = false
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var submitModel = new EventFormatViewModel { EventFormat = eventFormat };

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);

        var sut = new EventFormatController(sessionServiceMock.Object, validatorMock.Object);

        sut.Post(submitModel);

        sessionServiceMock.Verify(s => s.Set(It.Is<EventSessionModel>(m => m.HasChangedEvent == true)), Times.Once);
    }

    [Test]
    public void Post_EventFormat_HasSeenPreview_False_RedirectsToEventType()
    {
        var eventFormat = EventFormat.Hybrid;
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<EventFormatViewModel>>();

        var sessionModel = new EventSessionModel
        {
            HasSeenPreview = false
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var submitModel = new EventFormatViewModel { EventFormat = eventFormat };

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);

        var sut = new EventFormatController(sessionServiceMock.Object, validatorMock.Object);

        var result = (RedirectToRouteResult)sut.Post(submitModel);
        result.RouteName.Should().Be(RouteNames.CreateEvent.EventType);
    }

    [Test]
    public void Post_EventFormat_HasSeenPreview_True_RedirectsToLocation()
    {
        var eventFormat = EventFormat.Hybrid;
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<EventFormatViewModel>>();

        var sessionModel = new EventSessionModel
        {
            HasSeenPreview = true
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var submitModel = new EventFormatViewModel { EventFormat = eventFormat };

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);

        var sut = new EventFormatController(sessionServiceMock.Object, validatorMock.Object);

        var result = (RedirectToRouteResult)sut.Post(submitModel);
        result.RouteName.Should().Be(RouteNames.CreateEvent.Location);
    }

    [Test, MoqAutoData]
    public void Post_WhenNoSelectionOfEventFormat_Errors(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] EventFormatController sut)
    {
        sut.ModelState.AddModelError("key", "message");

        var submitModel = new EventFormatViewModel { CancelLink = NetworkEventsUrl };

        var result = (ViewResult)sut.Post(submitModel);

        sut.ModelState.IsValid.Should().BeFalse();
        Assert.That(result.Model, Is.TypeOf<EventFormatViewModel>());
        (result.Model as EventFormatViewModel)!.CancelLink.Should().Be(NetworkEventsUrl);
        sessionServiceMock.Verify(s => s.Set(It.IsAny<EventSessionModel>()), Times.Never());
    }
}