using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Controllers.ManageEvent;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;
using SFA.DAS.Admin.Aan.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers.ManageEvent;
public class SchoolEventControllerIsAtSchoolTests
{
    private static readonly string NetworkEventsUrl = Guid.NewGuid().ToString();
    private static readonly string PostUrl = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void Details_ReturnsEventAtSchoolViewModel(
        [Greedy] SchoolEventController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);
        var result = (ViewResult)sut.GetEventIsAtSchool();

        Assert.That(result.Model, Is.TypeOf<EventAtSchoolViewModel>());
        var vm = result.Model as EventAtSchoolViewModel;
        vm!.CancelLink.Should().Be(NetworkEventsUrl);
        vm.PageTitle.Should().Be(Application.Constants.CreateEvent.PageTitle);
    }

    [TestCase(true)]
    [TestCase(false)]
    [TestCase(null)]
    public void Details_ReturnsExpectedPostLink(bool? isAtSchool)
    {
        var sessionModel = new EventSessionModel { IsAtSchool = isAtSchool };
        var sessionServiceMock = new Mock<ISessionService>();
        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);
        var sut =
            new SchoolEventController(sessionServiceMock.Object, Mock.Of<IValidator<EventAtSchoolViewModel>>(), Mock.Of<IValidator<EventSchoolNameViewModel>>());
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.ManageEvent.EventIsAtSchool, PostUrl);
        var result = (ViewResult)sut.GetEventIsAtSchool();
        Assert.That(result.Model, Is.TypeOf<EventAtSchoolViewModel>());
        var vm = result.Model as EventAtSchoolViewModel;
        vm!.PostLink.Should().Be(PostUrl);
    }

    [TestCase(true)]
    [TestCase(false)]
    [TestCase(null)]
    public void Post_SetEventIsAtSchoolOnSessionModel(bool? isAtSchool)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<EventAtSchoolViewModel>>();

        var sessionModel = new EventSessionModel();

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var submitModel = new EventAtSchoolViewModel { IsAtSchool = isAtSchool };

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);

        var sut = new SchoolEventController(sessionServiceMock.Object, validatorMock.Object, Mock.Of<IValidator<EventSchoolNameViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);

        var result = (RedirectToRouteResult)sut.PostHasGuestSpeakers(submitModel);

        sut.ModelState.IsValid.Should().BeTrue();
        sessionServiceMock.Verify(s =>
            s.Set(It.Is<EventSessionModel>(m => m.IsAtSchool == isAtSchool)));
        if (isAtSchool == true)
        {
            result.RouteName.Should().Be(RouteNames.ManageEvent.EventSchoolName);
            result.RouteName.Should().Be(RouteNames.ManageEvent.EventSchoolName);
        }
        else
        {
            result.RouteName.Should().Be(RouteNames.ManageEvent.EventIsAtSchool);
        }
    }

    [Test, MoqAutoData]
    public void Post_WhenNoSelectionOfEventIsAtSchool_Errors(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SchoolEventController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);

        var sessionModel = new EventSessionModel();

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        sut.ModelState.AddModelError("key", "message");

        var submitModel = new EventAtSchoolViewModel();

        var result = (ViewResult)sut.PostHasGuestSpeakers(submitModel);

        sut.ModelState.IsValid.Should().BeFalse();
        Assert.That(result.Model, Is.TypeOf<EventAtSchoolViewModel>());
        (result.Model as EventAtSchoolViewModel)!.CancelLink.Should().Be(NetworkEventsUrl);
    }
}