using AutoFixture.NUnit3;
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
public class SchoolEventControllerIsAtSchoolTests
{
    private static readonly string NetworkEventsUrl = Guid.NewGuid().ToString();
    private static readonly string PostUrl = Guid.NewGuid().ToString();
    private static readonly string CheckYourAnswersUrl = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void GetEventIsAtSchool_ReturnsEventAtSchoolViewModel(
        [Greedy] SchoolEventController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);
        var result = (ViewResult)sut.GetEventIsAtSchool();

        Assert.That(result.Model, Is.TypeOf<IsAtSchoolViewModel>());
        var vm = result.Model as IsAtSchoolViewModel;
        vm!.CancelLink.Should().Be(NetworkEventsUrl);
        vm.PageTitle.Should().Be(Application.Constants.CreateEvent.PageTitle);
    }

    [TestCase(true)]
    [TestCase(false)]
    [TestCase(null)]
    public void GetEventIsAtSchool_ReturnsExpectedPostLink(bool? isAtSchool)
    {
        var sessionModel = new EventSessionModel { IsAtSchool = isAtSchool };
        var sessionServiceMock = new Mock<ISessionService>();
        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);
        var sut =
            new SchoolEventController(sessionServiceMock.Object, Mock.Of<IValidator<IsAtSchoolViewModel>>(), Mock.Of<IValidator<SchoolNameViewModel>>());
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.IsAtSchool, PostUrl);
        var result = (ViewResult)sut.GetEventIsAtSchool();
        Assert.That(result.Model, Is.TypeOf<IsAtSchoolViewModel>());
        var vm = result.Model as IsAtSchoolViewModel;
        vm!.PostLink.Should().Be(PostUrl);
    }

    [Test, MoqAutoData]
    public void Get_HasSeenPreviewFalse_CancelLinkIsManageEvents()
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<IsAtSchoolViewModel>>();

        var sessionModel = new EventSessionModel { HasSeenPreview = false };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new SchoolEventController(sessionServiceMock.Object, validatorMock.Object, Mock.Of<IValidator<SchoolNameViewModel>>());


        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);
        var result = (ViewResult)sut.GetEventIsAtSchool();

        Assert.That(result.Model, Is.TypeOf<IsAtSchoolViewModel>());
        var vm = result.Model as IsAtSchoolViewModel;
        vm!.CancelLink.Should().Be(NetworkEventsUrl);
    }

    [Test, MoqAutoData]
    public void Get_HasSeenPreviewTrue_CancelLinkIsCheckYourAnswers()
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<IsAtSchoolViewModel>>();

        var sessionModel = new EventSessionModel { HasSeenPreview = true };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new SchoolEventController(sessionServiceMock.Object, validatorMock.Object, Mock.Of<IValidator<SchoolNameViewModel>>());


        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.CheckYourAnswers, CheckYourAnswersUrl);
        var result = (ViewResult)sut.GetEventIsAtSchool();

        Assert.That(result.Model, Is.TypeOf<IsAtSchoolViewModel>());
        var vm = result.Model as IsAtSchoolViewModel;
        vm!.CancelLink.Should().Be(CheckYourAnswersUrl);
    }

    [TestCase(true)]
    [TestCase(false)]
    [TestCase(null)]
    public void Post_SetEventIsAtSchoolOnSessionModel(bool? isAtSchool)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<IsAtSchoolViewModel>>();

        var sessionModel = new EventSessionModel();

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var submitModel = new IsAtSchoolViewModel { IsAtSchool = isAtSchool };

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);

        var sut = new SchoolEventController(sessionServiceMock.Object, validatorMock.Object, Mock.Of<IValidator<SchoolNameViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);

        var result = (RedirectToRouteResult)sut.PostIsAtSchool(submitModel);

        sut.ModelState.IsValid.Should().BeTrue();
        sessionServiceMock.Verify(s =>
            s.Set(It.Is<EventSessionModel>(m => m.IsAtSchool == isAtSchool)));
        if (isAtSchool == true)
        {
            result.RouteName.Should().Be(RouteNames.CreateEvent.SchoolName);
        }
        else
        {
            result.RouteName.Should().Be(RouteNames.CreateEvent.OrganiserDetails);
        }
    }

    [Test]
    public void Post_HasSeenPreviewTrue_IsAtSchoolFalse_RedirectToCheckYourAnswers()
    {
        var isAtSchool = false;
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<IsAtSchoolViewModel>>();

        var sessionModel = new EventSessionModel { HasSeenPreview = true };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var submitModel = new IsAtSchoolViewModel { IsAtSchool = isAtSchool };

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);

        var sut = new SchoolEventController(sessionServiceMock.Object, validatorMock.Object, Mock.Of<IValidator<SchoolNameViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.CheckYourAnswers, CheckYourAnswersUrl);

        var result = (RedirectToRouteResult)sut.PostIsAtSchool(submitModel);

        sut.ModelState.IsValid.Should().BeTrue();
        sessionServiceMock.Verify(s =>
            s.Set(It.Is<EventSessionModel>(m => m.IsAtSchool == isAtSchool)));
        result.RouteName.Should().Be(RouteNames.CreateEvent.CheckYourAnswers);
    }

    [Test, MoqAutoData]
    public void Post_WhenNoSelectionOfEventIsAtSchool_Errors(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SchoolEventController sut)
    {
        sut.ModelState.AddModelError("key", "message");

        var submitModel = new IsAtSchoolViewModel { CancelLink = NetworkEventsUrl };

        var result = (ViewResult)sut.PostIsAtSchool(submitModel);

        sut.ModelState.IsValid.Should().BeFalse();
        Assert.That(result.Model, Is.TypeOf<IsAtSchoolViewModel>());
        (result.Model as IsAtSchoolViewModel)!.CancelLink.Should().Be(NetworkEventsUrl);
        sessionServiceMock.Verify(s => s.Set(It.IsAny<EventSessionModel>()), Times.Never());
    }
}