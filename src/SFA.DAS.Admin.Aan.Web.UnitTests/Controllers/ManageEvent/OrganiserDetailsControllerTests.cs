using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Admin.Aan.Application.Constants;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Controllers.ManageEvent;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;
using SFA.DAS.Admin.Aan.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers.ManageEvent;

public class OrganiserDetailsControllerTests
{
    private static readonly string AllNetworksUrl = Guid.NewGuid().ToString();
    private static readonly string PostUrl = Guid.NewGuid().ToString();
    private static readonly string CheckYourAnswersUrl = Guid.NewGuid().ToString();
    private static readonly string CalendarEventUrl = Guid.NewGuid().ToString();
    private static readonly string UpdateOrganiserDetailsUrl = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void Get_ReturnsCreateEventDetailsViewModel(
        [Greedy] OrganiserDetailsController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, AllNetworksUrl);
        var result = (ViewResult)sut.Get();

        Assert.That(result.Model, Is.TypeOf<OrganiserDetailsViewModel>());
        var vm = result.Model as OrganiserDetailsViewModel;
        vm!.CancelLink.Should().Be(AllNetworksUrl);
        vm.PageTitle.Should().Be(CreateEvent.PageTitle);
    }

    [Test, MoqAutoData]
    public void Get_ReturnsExpectedPostLink(
        [Greedy] OrganiserDetailsController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.OrganiserDetails, PostUrl);
        var result = (ViewResult)sut.Get();

        Assert.That(result.Model, Is.TypeOf<OrganiserDetailsViewModel>());
        var vm = result.Model as OrganiserDetailsViewModel;
        vm!.PostLink.Should().Be(PostUrl);
    }
    [Test, MoqAutoData]
    public void Get_ReturnsExpectedCancelLink_WhenHasSeenPreviewTrue()
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<OrganiserDetailsViewModel>>();

        var sessionModel = new EventSessionModel
        {
            HasSeenPreview = true
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new OrganiserDetailsController(sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.CheckYourAnswers, CheckYourAnswersUrl);
        var actualResult = sut.Get();
        var result = actualResult.As<ViewResult>();

        Assert.That(result.Model, Is.TypeOf<OrganiserDetailsViewModel>());
        var vm = result.Model as OrganiserDetailsViewModel;
        vm!.CancelLink.Should().Be(CheckYourAnswersUrl);
    }

    [Test, MoqAutoData]
    public void Get_IsAlreadyPublishedTrue_CancelLinkIsCalendarEvent()
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<OrganiserDetailsViewModel>>();

        var sessionModel = new EventSessionModel { IsAlreadyPublished = true };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new OrganiserDetailsController(sessionServiceMock.Object, validatorMock.Object);


        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CalendarEvent, CalendarEventUrl);
        var result = (ViewResult)sut.Get();

        Assert.That(result.Model, Is.TypeOf<OrganiserDetailsViewModel>());
        var vm = result.Model as OrganiserDetailsViewModel;
        vm!.CancelLink.Should().Be(CalendarEventUrl);
    }

    [TestCase("location 1", null)]
    [TestCase("location 2", "event online link")]
    [TestCase(null, "event online link")]
    public void Post_SetEventDetailsOnSessionModel(string? organiserName, string? organiserEmail)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<OrganiserDetailsViewModel>>();

        var sessionModel = new EventSessionModel();

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var submitModel = new OrganiserDetailsViewModel { OrganiserName = organiserName, OrganiserEmail = organiserEmail };

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);

        var sut = new OrganiserDetailsController(sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, AllNetworksUrl);

        var result = (RedirectToRouteResult)sut.Post(submitModel);

        sut.ModelState.IsValid.Should().BeTrue();
        sessionServiceMock.Verify(s => s.Set(It.Is<EventSessionModel>(m => m.ContactName == organiserName && m.ContactEmail == organiserEmail)));
        result.RouteName.Should().Be(RouteNames.CreateEvent.NumberOfAttendees);
    }

    [Test]
    public void Post_HasSeenPreview_False_RedirectsToNumberOfAttendees()
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<OrganiserDetailsViewModel>>();

        var sessionModel = new EventSessionModel();

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var submitModel = new OrganiserDetailsViewModel { OrganiserName = "org name", OrganiserEmail = "org@test.com" };

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);

        var sut = new OrganiserDetailsController(sessionServiceMock.Object, validatorMock.Object);
        var result = (RedirectToRouteResult)sut.Post(submitModel);
        result.RouteName.Should().Be(RouteNames.CreateEvent.NumberOfAttendees);
    }

    [Test]
    public void Post_HasSeenPreview_True_RedirectsToCheckYourAnswers()
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<OrganiserDetailsViewModel>>();

        var sessionModel = new EventSessionModel { HasSeenPreview = true };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);
        var submitModel = new OrganiserDetailsViewModel { OrganiserName = "org name", OrganiserEmail = "org@test.com" };

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);

        var sut = new OrganiserDetailsController(sessionServiceMock.Object, validatorMock.Object);
        var result = (RedirectToRouteResult)sut.Post(submitModel);
        result.RouteName.Should().Be(RouteNames.CreateEvent.CheckYourAnswers);
    }

    [Test, MoqAutoData]
    public void Get_IsAlreadyPublishedTrue_PostLinkIsUpdateCalendarEvent()
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<OrganiserDetailsViewModel>>();

        var sessionModel = new EventSessionModel { IsAlreadyPublished = true };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new OrganiserDetailsController(sessionServiceMock.Object, validatorMock.Object);


        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.UpdateEvent.UpdateOrganiserDetails, UpdateOrganiserDetailsUrl);
        var result = (ViewResult)sut.Get();

        Assert.That(result.Model, Is.TypeOf<OrganiserDetailsViewModel>());
        var vm = result.Model as OrganiserDetailsViewModel;
        vm!.PostLink.Should().Be(UpdateOrganiserDetailsUrl);
    }

    [Test, MoqAutoData]
    public void Post_IsAlreadyPublishedTrue_RedirectsToCalendarEvent()
    {
        var calendarEventId = Guid.NewGuid();
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<OrganiserDetailsViewModel>>();

        var sessionModel = new EventSessionModel
        {
            CalendarEventId = calendarEventId,
            IsAlreadyPublished = true
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var submitModel = new OrganiserDetailsViewModel();

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);

        var sut = new OrganiserDetailsController(sessionServiceMock.Object, validatorMock.Object);

        var result = (RedirectToRouteResult)sut.Post(submitModel);
        result.RouteName.Should().Be(RouteNames.CalendarEvent);
    }

    [Test, MoqAutoData]
    public void Post_IsAlreadyPublishedTrue_SetsHasChangedEventToTrue()
    {
        var calendarEventId = Guid.NewGuid();
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<OrganiserDetailsViewModel>>();

        var sessionModel = new EventSessionModel
        {
            CalendarEventId = calendarEventId,
            IsAlreadyPublished = true,
            HasChangedEvent = false
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var submitModel = new OrganiserDetailsViewModel();

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);

        var sut = new OrganiserDetailsController(sessionServiceMock.Object, validatorMock.Object);

        sut.Post(submitModel);
        sessionServiceMock.Verify(s => s.Set(It.Is<EventSessionModel>(m => m.HasChangedEvent == true)), Times.Once);
    }

    [Test, MoqAutoData]
    public void Post_WhenNoSelectionOfEventLocation_Errors(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] OrganiserDetailsController sut)
    {
        sut.ModelState.AddModelError("key", "message");

        var submitModel = new OrganiserDetailsViewModel { CancelLink = AllNetworksUrl };
        var result = (ViewResult)sut.Post(submitModel);

        sut.ModelState.IsValid.Should().BeFalse();
        Assert.That(result.Model, Is.TypeOf<OrganiserDetailsViewModel>());
        (result.Model as OrganiserDetailsViewModel)!.CancelLink.Should().Be(AllNetworksUrl);
        sessionServiceMock.Verify(s => s.Set(It.IsAny<EventSessionModel>()), Times.Never());
    }
}