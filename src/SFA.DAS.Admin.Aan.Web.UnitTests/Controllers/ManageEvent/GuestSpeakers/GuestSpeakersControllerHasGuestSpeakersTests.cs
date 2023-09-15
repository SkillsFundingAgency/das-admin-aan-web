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

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers.ManageEvent.GuestSpeakers;
public class GuestSpeakersControllerHasGuestSpeakersTests
{
    private static readonly string NetworkEventsUrl = Guid.NewGuid().ToString();
    private static readonly string PostUrl = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void Details_ReturnsEventGuestSpeakerViewModel(
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
    public void Details_ReturnsExpectedPostLink(
        [Greedy] GuestSpeakersController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.ManageEvent.EventHasGuestSpeakers, PostUrl);
        var result = (ViewResult)sut.GetHasGuestSpeakers();
        Assert.That(result.Model, Is.TypeOf<HasGuestSpeakersViewModel>());
        var vm = result.Model as HasGuestSpeakersViewModel;
        vm!.PostLink.Should().Be(PostUrl);
    }

    [TestCase(true)]
    [TestCase(false)]
    [TestCase(null)]
    public void Post_SetHasGuestSpeakersOnSessionModel(bool? hasGuestSpeakers)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<HasGuestSpeakersViewModel>>();

        var sessionModel = new EventSessionModel();

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
            result.RouteName.Should().Be(RouteNames.ManageEvent.GuestSpeakerList);
        }
        else
        {
            result.RouteName.Should().Be(RouteNames.ManageEvent.EventHasGuestSpeakers);
        }
    }

    [Test, MoqAutoData]
    public void Post_WhenNoSelectionOfHasGuestSpeakers_Errors(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] GuestSpeakersController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);

        var sessionModel = new EventSessionModel();

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        sut.ModelState.AddModelError("key", "message");

        var submitModel = new HasGuestSpeakersViewModel();

        var result = (ViewResult)sut.PostHasGuestSpeakers(submitModel);

        sut.ModelState.IsValid.Should().BeFalse();
        Assert.That(result.Model, Is.TypeOf<HasGuestSpeakersViewModel>());
        (result.Model as HasGuestSpeakersViewModel)!.CancelLink.Should().Be(NetworkEventsUrl);
    }
}