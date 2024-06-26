﻿using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Controllers.ManageEvent;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;
using SFA.DAS.Admin.Aan.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers.ManageEvent.GuestSpeakers;
public class GuestSpeakersControllerHasGuestSpeakersTests
{
    private static readonly string NetworkEventsUrl = Guid.NewGuid().ToString();
    private static readonly string PostUrl = Guid.NewGuid().ToString();
    private static readonly string CheckYourAnswersUrl = Guid.NewGuid().ToString();
    private static readonly string CalendarEventUrl = Guid.NewGuid().ToString();
    private static readonly string UpdateHasGuestSpeakersUrl = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void Get_ReturnsEventGuestSpeakerViewModel(
        [Greedy] GuestSpeakersController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);
        var result = (ViewResult)sut.GetHasGuestSpeakers();

        Assert.That(result.Model, Is.TypeOf<HasGuestSpeakersViewModel>());
        var vm = result.Model as HasGuestSpeakersViewModel;
        vm!.CancelLink.Should().Be(NetworkEventsUrl);
        vm.PageTitle.Should().Be(Application.Constants.CreateEvent.PageTitle);
    }

    [Test, MoqAutoData]
    public void Get_ReturnsExpectedPostLink(
        [Greedy] GuestSpeakersController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.HasGuestSpeakers, PostUrl);
        var result = (ViewResult)sut.GetHasGuestSpeakers();
        Assert.That(result.Model, Is.TypeOf<HasGuestSpeakersViewModel>());
        var vm = result.Model as HasGuestSpeakersViewModel;
        vm!.PostLink.Should().Be(PostUrl);
    }

    [Test, MoqAutoData]
    public void Get_PostLinkIsUpdateHasGuestSpeakersLink_WhenIsAlreadyPublishedTrue()
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<HasGuestSpeakersViewModel>>();

        var sessionModel = new EventSessionModel { IsAlreadyPublished = true, HasGuestSpeakers = false };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new GuestSpeakersController(sessionServiceMock.Object, Mock.Of<IValidator<GuestSpeakerAddViewModel>>(), validatorMock.Object);


        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.UpdateEvent.UpdateHasGuestSpeakers, UpdateHasGuestSpeakersUrl);
        var result = (ViewResult)sut.GetHasGuestSpeakers();

        Assert.That(result.Model, Is.TypeOf<HasGuestSpeakersViewModel>());
        var vm = result.Model as HasGuestSpeakersViewModel;
        vm!.PostLink.Should().Be(UpdateHasGuestSpeakersUrl);
    }

    [Test, MoqAutoData]
    public void Get_ReturnsExpectedCancelLink_WhenHasSeenPreviewTrue()
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<HasGuestSpeakersViewModel>>();

        var sessionModel = new EventSessionModel
        {
            HasSeenPreview = true
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new GuestSpeakersController(sessionServiceMock.Object, Mock.Of<IValidator<GuestSpeakerAddViewModel>>(), validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.CheckYourAnswers, CheckYourAnswersUrl);
        var actualResult = sut.GetHasGuestSpeakers();
        var result = actualResult.As<ViewResult>();

        Assert.That(result.Model, Is.TypeOf<HasGuestSpeakersViewModel>());
        var vm = result.Model as HasGuestSpeakersViewModel;
        vm!.CancelLink.Should().Be(CheckYourAnswersUrl);
    }

    [Test, MoqAutoData]
    public void Get_ReturnsExpectedCancelLink_WhenIsAlreadyPublished()
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<HasGuestSpeakersViewModel>>();

        var sessionModel = new EventSessionModel
        {
            IsAlreadyPublished = true,
            HasSeenPreview = true,
            HasGuestSpeakers = false
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new GuestSpeakersController(sessionServiceMock.Object, Mock.Of<IValidator<GuestSpeakerAddViewModel>>(), validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CalendarEvent, CalendarEventUrl);
        var actualResult = sut.GetHasGuestSpeakers();
        var result = actualResult.As<ViewResult>();

        Assert.That(result.Model, Is.TypeOf<HasGuestSpeakersViewModel>());
        var vm = result.Model as HasGuestSpeakersViewModel;
        vm!.CancelLink.Should().Be(CalendarEventUrl);
    }

    [TestCase(true, false)]
    [TestCase(false, false)]
    [TestCase(null, false)]
    [TestCase(true, true)]
    [TestCase(false, true)]
    public void Post_SetHasGuestSpeakersOnSessionModel(bool? hasGuestSpeakers, bool hasSeenPreview)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<HasGuestSpeakersViewModel>>();

        var sessionModel = new EventSessionModel { HasSeenPreview = hasSeenPreview };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var submitModel = new HasGuestSpeakersViewModel() { HasGuestSpeakers = hasGuestSpeakers };

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);

        var sut = new GuestSpeakersController(sessionServiceMock.Object, Mock.Of<IValidator<GuestSpeakerAddViewModel>>(), validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);

        var result = (RedirectToRouteResult)sut.PostHasGuestSpeakers(submitModel);

        sut.ModelState.IsValid.Should().BeTrue();
        sessionServiceMock.Verify(s =>
            s.Set(It.Is<EventSessionModel>(m => m.HasGuestSpeakers == hasGuestSpeakers)));
        if (hasGuestSpeakers == true)
        {
            result.RouteName.Should().Be(RouteNames.CreateEvent.GuestSpeakerList);
        }
        else
        {
            result.RouteName.Should().Be(hasSeenPreview
                ? RouteNames.CreateEvent.CheckYourAnswers
                : RouteNames.CreateEvent.DateAndTime);
        }
    }

    [Test, MoqAutoData]
    public void Post_IsAlreadyPublishedTrueHasGuestSpeakersFalse_RedirectsToCalendarEvent()
    {
        var calendarEventId = Guid.NewGuid();
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<HasGuestSpeakersViewModel>>();

        var sessionModel = new EventSessionModel
        {
            CalendarEventId = calendarEventId,
            IsAlreadyPublished = true,
            HasGuestSpeakers = false
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var submitModel = new HasGuestSpeakersViewModel();

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);

        var sut = new GuestSpeakersController(sessionServiceMock.Object, Mock.Of<IValidator<GuestSpeakerAddViewModel>>(), validatorMock.Object);

        var result = (RedirectToRouteResult)sut.PostHasGuestSpeakers(submitModel);
        result.RouteName.Should().Be(RouteNames.CalendarEvent);
    }

    [Test, MoqAutoData]
    public void Post_IsAlreadyPublishedTrue_RedirectsToCalendarEvent()
    {
        var calendarEventId = Guid.NewGuid();
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<HasGuestSpeakersViewModel>>();

        var sessionModel = new EventSessionModel
        {
            CalendarEventId = calendarEventId,
            IsAlreadyPublished = true,
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var submitModel = new HasGuestSpeakersViewModel { HasGuestSpeakers = true };

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);

        var sut = new GuestSpeakersController(sessionServiceMock.Object, Mock.Of<IValidator<GuestSpeakerAddViewModel>>(), validatorMock.Object);

        var result = (RedirectToRouteResult)sut.PostHasGuestSpeakers(submitModel);
        result.RouteName.Should().Be(RouteNames.UpdateEvent.UpdateGuestSpeakerList);
    }

    [Test, MoqAutoData]
    public void Post_IsAlreadyPublishedTrue_SetsHasChangedEventToTrue()
    {
        var calendarEventId = Guid.NewGuid();
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<HasGuestSpeakersViewModel>>();

        var sessionModel = new EventSessionModel
        {
            CalendarEventId = calendarEventId,
            IsAlreadyPublished = true
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var submitModel = new HasGuestSpeakersViewModel { HasGuestSpeakers = true };

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);

        var sut = new GuestSpeakersController(sessionServiceMock.Object, Mock.Of<IValidator<GuestSpeakerAddViewModel>>(), validatorMock.Object);

        sut.PostHasGuestSpeakers(submitModel);
        sessionServiceMock.Verify(s => s.Set(It.Is<EventSessionModel>(m => m.HasChangedEvent == true)), Times.Once);
    }

    [Test, MoqAutoData]
    public void Post_WhenNoSelectionOfHasGuestSpeakers_Errors(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] GuestSpeakersController sut)
    {
        sut.ModelState.AddModelError("key", "message");

        var submitModel = new HasGuestSpeakersViewModel { CancelLink = NetworkEventsUrl };

        var result = (ViewResult)sut.PostHasGuestSpeakers(submitModel);

        sut.ModelState.IsValid.Should().BeFalse();
        Assert.That(result.Model, Is.TypeOf<HasGuestSpeakersViewModel>());
        (result.Model as HasGuestSpeakersViewModel)!.CancelLink.Should().Be(NetworkEventsUrl);
        sessionServiceMock.Verify(s => s.Set(It.IsAny<EventSessionModel>()), Times.Never());
    }
}