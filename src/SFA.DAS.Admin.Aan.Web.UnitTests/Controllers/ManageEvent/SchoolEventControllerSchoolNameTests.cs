﻿using AutoFixture.NUnit3;
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

public class SchoolEventControllerSchoolNameTests
{
    private static readonly string NetworkEventsUrl = Guid.NewGuid().ToString();
    private static readonly string PostUrl = Guid.NewGuid().ToString();
    private static readonly string CheckYourAnswersUrl = Guid.NewGuid().ToString();
    private static readonly string CalendarEventUrl = Guid.NewGuid().ToString();
    private static readonly string SchoolNameUrl = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void Get_ReturnsEventAtSchoolViewModel(
        [Greedy] SchoolEventController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);
        var result = (ViewResult)sut.GetSchoolName();

        Assert.That(result.Model, Is.TypeOf<SchoolNameViewModel>());
        var vm = result.Model as SchoolNameViewModel;
        vm!.CancelLink.Should().Be(NetworkEventsUrl);
        vm.PageTitle.Should().Be(CreateEvent.PageTitle);
        vm.SearchResult.Should().Be(" (URN: )");
    }

    [Test, MoqAutoData]
    public void Get_ReturnsExpectedPostLink(
        [Greedy] SchoolEventController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.SchoolName, PostUrl);
        var result = (ViewResult)sut.GetSchoolName();

        Assert.That(result.Model, Is.TypeOf<SchoolNameViewModel>());
        var vm = result.Model as SchoolNameViewModel;
        vm!.PostLink.Should().Be(PostUrl);
    }

    [Test]
    public void Get_ReturnsExpectedPostLink_WhenIsAlreadyPublishedTrue()
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<SchoolNameViewModel>>();

        var sessionModel = new EventSessionModel { IsAlreadyPublished = true };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new SchoolEventController(sessionServiceMock.Object, Mock.Of<IValidator<IsAtSchoolViewModel>>(), validatorMock.Object);


        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.UpdateEvent.UpdateSchoolName, SchoolNameUrl);
        var result = (ViewResult)sut.GetSchoolName();

        Assert.That(result.Model, Is.TypeOf<SchoolNameViewModel>());
        var vm = result.Model as SchoolNameViewModel;
        vm!.PostLink.Should().Be(SchoolNameUrl);
    }

    [Test]
    public void Get_HasSeenPreviewTrue_CancelLinkIsCheckYourAnswers()
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<SchoolNameViewModel>>();

        var sessionModel = new EventSessionModel { HasSeenPreview = true };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new SchoolEventController(sessionServiceMock.Object, Mock.Of<IValidator<IsAtSchoolViewModel>>(), validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.CheckYourAnswers, CheckYourAnswersUrl);
        var result = (ViewResult)sut.GetSchoolName();

        Assert.That(result.Model, Is.TypeOf<SchoolNameViewModel>());
        var vm = result.Model as SchoolNameViewModel;
        vm!.CancelLink.Should().Be(CheckYourAnswersUrl);
    }

    [Test]
    public void Get_IsAlreadyPublishedTrue_CancelLinkIsCalendarEvent()
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<SchoolNameViewModel>>();

        var sessionModel = new EventSessionModel { IsAlreadyPublished = true };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new SchoolEventController(sessionServiceMock.Object, Mock.Of<IValidator<IsAtSchoolViewModel>>(), validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CalendarEvent, CalendarEventUrl);
        var result = (ViewResult)sut.GetSchoolName();

        Assert.That(result.Model, Is.TypeOf<SchoolNameViewModel>());
        var vm = result.Model as SchoolNameViewModel;
        vm!.CancelLink.Should().Be(CalendarEventUrl);
    }

    [Test]
    public void Get_HasSeenPreviewFalse_CancelLinkIsManageEvents()
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<SchoolNameViewModel>>();

        var sessionModel = new EventSessionModel { HasSeenPreview = false };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new SchoolEventController(sessionServiceMock.Object, Mock.Of<IValidator<IsAtSchoolViewModel>>(), validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);
        var result = (ViewResult)sut.GetEventIsAtSchool();

        Assert.That(result.Model, Is.TypeOf<IsAtSchoolViewModel>());
        var vm = result.Model as IsAtSchoolViewModel;
        vm!.CancelLink.Should().Be(NetworkEventsUrl);
    }

    [TestCase("name 1", "urn 1", "name 1 (URN: urn 1)")]
    [TestCase(null, "urn 1", " (URN: urn 1)")]
    [TestCase("name 1", null, "name 1 (URN: )")]
    [TestCase(null, null, " (URN: )")]
    public void Details_ReturnsExpectedSearchTerm(string? schoolName, string? urn, string expectedResult)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var sessionModel = new EventSessionModel { SchoolName = schoolName, Urn = urn };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new SchoolEventController(sessionServiceMock.Object, Mock.Of<IValidator<IsAtSchoolViewModel>>(), Mock.Of<IValidator<SchoolNameViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.SchoolName, PostUrl);
        var result = (ViewResult)sut.GetSchoolName();

        Assert.That(result.Model, Is.TypeOf<SchoolNameViewModel>());
        var vm = result.Model as SchoolNameViewModel;
        vm!.PostLink.Should().Be(PostUrl);
        vm.SearchResult.Should().Be(expectedResult);
    }

    [TestCase("12345")]
    [TestCase("54321")]
    public void Post_SetEventDetailsOnSessionModel(string urn)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<SchoolNameViewModel>>();

        var sessionModel = new EventSessionModel();

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var submitModel = new SchoolNameViewModel { Urn = urn };

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);

        var sut = new SchoolEventController(sessionServiceMock.Object, Mock.Of<IValidator<IsAtSchoolViewModel>>(), validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);

        var result = (RedirectToRouteResult)sut.PostSchoolName(submitModel);

        sut.ModelState.IsValid.Should().BeTrue();
        sessionServiceMock.Verify(s => s.Set(It.Is<EventSessionModel>(m => m.Urn == urn)));
        result.RouteName.Should().Be(RouteNames.CreateEvent.OrganiserDetails);
    }

    [Test]
    public void Post_HasSeenPreviewTrue_RedirectToCheckYourAnswers()
    {
        var schoolName = "school name";
        var urn = "32123";
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<SchoolNameViewModel>>();

        var sessionModel = new EventSessionModel { HasSeenPreview = true };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var submitModel = new SchoolNameViewModel { Urn = urn, Name = schoolName };

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);

        var sut = new SchoolEventController(sessionServiceMock.Object, Mock.Of<IValidator<IsAtSchoolViewModel>>(), validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.CheckYourAnswers, CheckYourAnswersUrl);

        var result = (RedirectToRouteResult)sut.PostSchoolName(submitModel);

        sut.ModelState.IsValid.Should().BeTrue();
        sessionServiceMock.Verify(s =>
            s.Set(It.Is<EventSessionModel>(m => m.SchoolName == schoolName && m.Urn == urn)));
        result.RouteName.Should().Be(RouteNames.CreateEvent.CheckYourAnswers);
    }

    [Test]
    public void Post_IsAlreadyPublishedTrue_RedirectToCalendarEvent()
    {
        var schoolName = "school name";
        var urn = "32123";
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<SchoolNameViewModel>>();

        var sessionModel = new EventSessionModel { IsAlreadyPublished = true };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var submitModel = new SchoolNameViewModel { Urn = urn, Name = schoolName };

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);

        var sut = new SchoolEventController(sessionServiceMock.Object, Mock.Of<IValidator<IsAtSchoolViewModel>>(), validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CalendarEvent, CalendarEventUrl);

        var result = (RedirectToRouteResult)sut.PostSchoolName(submitModel);

        sut.ModelState.IsValid.Should().BeTrue();
        sessionServiceMock.Verify(s =>
            s.Set(It.Is<EventSessionModel>(m => m.SchoolName == schoolName && m.Urn == urn)));
        result.RouteName.Should().Be(RouteNames.CalendarEvent);
    }

    [Test]
    public void Post_IsAlreadyPublishedTrue_SetsHasChangedEventToTrue()
    {
        var schoolName = "school name";
        var urn = "32123";
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<SchoolNameViewModel>>();

        var sessionModel = new EventSessionModel { IsAlreadyPublished = true };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var submitModel = new SchoolNameViewModel { Urn = urn, Name = schoolName };

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);

        var sut = new SchoolEventController(sessionServiceMock.Object, Mock.Of<IValidator<IsAtSchoolViewModel>>(), validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CalendarEvent, CalendarEventUrl);

        sut.PostSchoolName(submitModel);
        sessionServiceMock.Verify(s => s.Set(It.Is<EventSessionModel>(m => m.HasChangedEvent == true)), Times.Once);
    }

    [Test, MoqAutoData]
    public void Post_WhenNoSelectionOfEventLocation_Errors(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SchoolEventController sut)
    {
        sut.ModelState.AddModelError("key", "message");

        var submitModel = new SchoolNameViewModel { CancelLink = NetworkEventsUrl };

        var result = (ViewResult)sut.PostSchoolName(submitModel);

        sut.ModelState.IsValid.Should().BeFalse();
        Assert.That(result.Model, Is.TypeOf<SchoolNameViewModel>());
        (result.Model as SchoolNameViewModel)!.CancelLink.Should().Be(NetworkEventsUrl);
        sessionServiceMock.Verify(s => s.Set(It.IsAny<EventSessionModel>()), Times.Never());
    }
}