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

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers.ManageEvent;
public class NumberOfAttendeesControllerTests
{
    private static readonly string NetworkEventsUrl = Guid.NewGuid().ToString();
    private static readonly string PostUrl = Guid.NewGuid().ToString();
    private static readonly string CheckYourAnswersUrl = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void Get_ReturnsNumberOfAttendeesViewModel(
        [Greedy] NumberOfAttendeesController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);
        var result = (ViewResult)sut.Get();

        Assert.That(result.Model, Is.TypeOf<NumberOfAttendeesViewModel>());
        var vm = result.Model as NumberOfAttendeesViewModel;
        vm!.CancelLink.Should().Be(NetworkEventsUrl);
        vm.PageTitle.Should().Be(Application.Constants.CreateEvent.PageTitle);
    }

    [Test, MoqAutoData]
    public void Get_ReturnsExpectedPostLink(
        [Greedy] NumberOfAttendeesController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.ManageEvent.NumberOfAttendees, PostUrl);
        var result = (ViewResult)sut.Get();
        Assert.That(result.Model, Is.TypeOf<NumberOfAttendeesViewModel>());
        var vm = result.Model as NumberOfAttendeesViewModel;
        vm!.PostLink.Should().Be(PostUrl);
    }

    [Test, MoqAutoData]
    public void Get_ReturnsExpectedCancelLink_WhenHasSeenPreviewTrue()
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<NumberOfAttendeesViewModel>>();

        var sessionModel = new EventSessionModel
        {
            HasSeenPreview = true
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new NumberOfAttendeesController(sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.ManageEvent.CheckYourAnswers, CheckYourAnswersUrl);
        var actualResult = sut.Get();
        var result = actualResult.As<ViewResult>();

        Assert.That(result.Model, Is.TypeOf<NumberOfAttendeesViewModel>());
        var vm = result.Model as NumberOfAttendeesViewModel;
        vm!.CancelLink.Should().Be(CheckYourAnswersUrl);
    }

    [TestCase(1)]
    [TestCase(12)]
    [TestCase(1000000)]
    public void Post_SetEventNumberOfAttendeesOnSessionModel(int numberOfAttendees)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<NumberOfAttendeesViewModel>>();

        var sessionModel = new EventSessionModel();

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var submitModel = new NumberOfAttendeesViewModel
        {
            NumberOfAttendees = numberOfAttendees
        };

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);

        var sut = new NumberOfAttendeesController(sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);

        var result = (RedirectToRouteResult)sut.Post(submitModel);

        sut.ModelState.IsValid.Should().BeTrue();
        sessionServiceMock.Verify(s => s.Set(It.Is<EventSessionModel>(m
            => m.PlannedAttendees == numberOfAttendees)));
        result.RouteName.Should().Be(RouteNames.ManageEvent.CheckYourAnswers);
    }

    [Test, MoqAutoData]
    public void Post_WhenNoSelectionOfEventNumberOfAttendees_Errors(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] NumberOfAttendeesController sut)
    {
        sut.ModelState.AddModelError("key", "message");

        var submitModel = new NumberOfAttendeesViewModel { CancelLink = NetworkEventsUrl };

        var result = (ViewResult)sut.Post(submitModel);

        sut.ModelState.IsValid.Should().BeFalse();
        Assert.That(result.Model, Is.TypeOf<NumberOfAttendeesViewModel>());
        (result.Model as NumberOfAttendeesViewModel)!.CancelLink.Should().Be(NetworkEventsUrl);
        sessionServiceMock.Verify(s => s.Set(It.IsAny<EventSessionModel>()), Times.Never());
    }
}
