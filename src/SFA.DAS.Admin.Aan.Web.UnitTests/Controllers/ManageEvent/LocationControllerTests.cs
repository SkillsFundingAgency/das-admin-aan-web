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

public class LocationControllerTests
{
    private static readonly string AllNetworksUrl = Guid.NewGuid().ToString();
    private static readonly string PostUrl = Guid.NewGuid().ToString();
    private static readonly string CheckYourAnswersUrl = Guid.NewGuid().ToString();
    private static readonly string CalendarEventUrl = Guid.NewGuid().ToString();
    private static readonly string UpdateLocationUrl = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void Get_ReturnsLocationViewModel(
        [Greedy] LocationController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, AllNetworksUrl);
        var result = (ViewResult)sut.Get();

        Assert.That(result.Model, Is.TypeOf<LocationViewModel>());
        var vm = result.Model as LocationViewModel;
        vm!.CancelLink.Should().Be(AllNetworksUrl);
        vm.PageTitle.Should().Be(CreateEvent.PageTitle);
    }

    [Test, MoqAutoData]
    public void Get_HasSeenPreviewTrue_CancelLinkIsCheckYourAnswers()
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<LocationViewModel>>();

        var sessionModel = new EventSessionModel { HasSeenPreview = true };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new LocationController(sessionServiceMock.Object, validatorMock.Object);


        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.CheckYourAnswers, CheckYourAnswersUrl);
        var result = (ViewResult)sut.Get();

        Assert.That(result.Model, Is.TypeOf<LocationViewModel>());
        var vm = result.Model as LocationViewModel;
        vm!.CancelLink.Should().Be(CheckYourAnswersUrl);
    }

    [Test, MoqAutoData]
    public void Get_IsAlreadyPublishedTrue_CancelLinkIsCalendarEvent()
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<LocationViewModel>>();

        var sessionModel = new EventSessionModel { IsAlreadyPublished = true };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new LocationController(sessionServiceMock.Object, validatorMock.Object);


        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CalendarEvent, CalendarEventUrl);
        var result = (ViewResult)sut.Get();

        Assert.That(result.Model, Is.TypeOf<LocationViewModel>());
        var vm = result.Model as LocationViewModel;
        vm!.CancelLink.Should().Be(CalendarEventUrl);
    }

    [Test, MoqAutoData]
    public void Get_ReturnsExpectedPostLink(
        [Greedy] LocationController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.Location, PostUrl);
        var result = (ViewResult)sut.Get();

        Assert.That(result.Model, Is.TypeOf<LocationViewModel>());
        var vm = result.Model as LocationViewModel;
        vm!.PostLink.Should().Be(PostUrl);
    }

    [Test, MoqAutoData]
    public void Get_IsAlreadyPublishedTrue_PostLinkLinkIsUpdateCalendarEvent()
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<LocationViewModel>>();

        var sessionModel = new EventSessionModel { IsAlreadyPublished = true };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new LocationController(sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.UpdateEvent.UpdateLocation, UpdateLocationUrl);
        var result = (ViewResult)sut.Get();

        Assert.That(result.Model, Is.TypeOf<LocationViewModel>());
        var vm = result.Model as LocationViewModel;
        vm!.PostLink.Should().Be(UpdateLocationUrl);
    }
    [TestCase(true, UpdateEvent.PageTitle)]
    [TestCase(false, CreateEvent.PageTitle)]
    public void Get_ReturnsExpectedPageTitle(bool isAlreadyPublished, string pageTitle)
    {
        var calendarEventId = Guid.NewGuid();
        var sessionServiceMock = new Mock<ISessionService>();
        var sessionModel = new EventSessionModel { IsAlreadyPublished = isAlreadyPublished, CalendarEventId = calendarEventId };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new LocationController(sessionServiceMock.Object, Mock.Of<IValidator<LocationViewModel>>());
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CalendarEvent, CalendarEventUrl);

        var result = (ViewResult)sut.Get();

        Assert.That(result.Model, Is.TypeOf<LocationViewModel>());
        var vm = result.Model as LocationViewModel;
        vm!.PageTitle.Should().Be(pageTitle);
    }

    [Test, MoqAutoData]
    public void Post_IsAlreadyPublishedTrue_RedirectsToUpdateLocation()
    {
        var calendarEventId = Guid.NewGuid();
        //var eventFormat = EventFormat.Hybrid;
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<LocationViewModel>>();

        var sessionModel = new EventSessionModel
        {
            CalendarEventId = calendarEventId,
            IsAlreadyPublished = true
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var submitModel = new LocationViewModel();

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);

        var sut = new LocationController(sessionServiceMock.Object, validatorMock.Object);

        var result = (RedirectToRouteResult)sut.Post(submitModel);
        result.RouteName.Should().Be(RouteNames.CalendarEvent);
        result.RouteValues!["CalendarEventId"].Should().Be(calendarEventId);
    }

    [TestCase("AB1 CTX", null, null)]
    [TestCase("AB2 CTX", "event online link", null)]
    [TestCase(null, "event online link", EventFormat.Hybrid)]
    public void Post_SetEventDetailsOnSessionModel(string? postcode, string? eventOnlineLink, EventFormat? eventFormat)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<LocationViewModel>>();

        var sessionModel = new EventSessionModel
        {
            HasSeenPreview = false
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var submitModel = new LocationViewModel { Postcode = postcode, OnlineEventLink = eventOnlineLink, EventFormat = eventFormat };

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);

        var sut = new LocationController(sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, AllNetworksUrl);

        var result = (RedirectToRouteResult)sut.Post(submitModel);

        sut.ModelState.IsValid.Should().BeTrue();
        sessionServiceMock.Verify(s => s.Set(It.Is<EventSessionModel>(m => m.Postcode == postcode && m.EventLink == eventOnlineLink)));
        result.RouteName.Should().Be(submitModel.EventFormat == null
            ? RouteNames.CreateEvent.OrganiserDetails
            : RouteNames.CreateEvent.IsAtSchool);
    }


    [Test]
    public void Post_Set_HasSeenPreview_True()
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<LocationViewModel>>();

        var sessionModel = new EventSessionModel
        {
            HasSeenPreview = true
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var submitModel = new LocationViewModel { Postcode = "LE12 1QW", OnlineEventLink = "https://www.google.com", EventFormat = EventFormat.Hybrid };

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);

        var sut = new LocationController(sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, AllNetworksUrl);

        var result = (RedirectToRouteResult)sut.Post(submitModel);

        sut.ModelState.IsValid.Should().BeTrue();
        result.RouteName.Should().Be(RouteNames.CreateEvent.CheckYourAnswers);
    }

    [Test]
    public void Post_IsAlreadyPublishedTrue_SetsHasChangedEventToTrue()
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<LocationViewModel>>();

        var sessionModel = new EventSessionModel
        {
            HasChangedEvent = true
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var submitModel = new LocationViewModel { Postcode = "LE12 1QW", OnlineEventLink = "https://www.google.com", EventFormat = EventFormat.Hybrid };

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);

        var sut = new LocationController(sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, AllNetworksUrl);

        sut.Post(submitModel);

        sut.ModelState.IsValid.Should().BeTrue();
        sessionServiceMock.Verify(s => s.Set(It.Is<EventSessionModel>(m => m.HasChangedEvent == true)), Times.Once);
    }

    [Test, MoqAutoData]
    public void Post_WhenNoSelectionOfEventLocation_Errors(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] LocationController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, AllNetworksUrl);
        sut.ModelState.AddModelError("key", "message");

        var submitModel = new LocationViewModel { CancelLink = AllNetworksUrl };

        submitModel.EventOnlineLinkMaxLength.Should().Be(ManageEventValidation.EventOnlineLinkMaxLength);

        var result = (ViewResult)sut.Post(submitModel);

        sut.ModelState.IsValid.Should().BeFalse();
        Assert.That(result.Model, Is.TypeOf<LocationViewModel>());
        (result.Model as LocationViewModel)!.CancelLink.Should().Be(AllNetworksUrl);
        sessionServiceMock.Verify(s => s.Set(It.IsAny<EventSessionModel>()), Times.Never());
    }

    [Test]
    public void Post_SetLocationDetailsOnSessionModel_InPerson_CheckEventLinkIsSetToNull()
    {
        var eventFormat = EventFormat.InPerson;
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<LocationViewModel>>();

        var sessionModel = new EventSessionModel
        {
            EventLink = "https://www.google.com",
            EventFormat = eventFormat
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var submitModel = new LocationViewModel { EventFormat = eventFormat };

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);

        var sut = new LocationController(sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, AllNetworksUrl);

        var result = (RedirectToRouteResult)sut.Post(submitModel);

        sut.ModelState.IsValid.Should().BeTrue();
        sessionServiceMock.Verify(s => s.Set(It.Is<EventSessionModel>(m
            => m.EventFormat == eventFormat
               && m.EventLink == null
        )));

        result.RouteName.Should().Be(RouteNames.CreateEvent.IsAtSchool);
    }
}