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
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;
using SFA.DAS.Admin.Aan.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers.ManageEvent;

public class NetworkEventLocationControllerTests
{
    private static readonly string AllNetworksUrl = Guid.NewGuid().ToString();
    private static readonly string PostUrl = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void Details_ReturnsCreateEventDetailsViewModel(
        [Greedy] NetworkEventLocationController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, AllNetworksUrl);
        var result = (ViewResult)sut.Get();

        Assert.That(result.Model, Is.TypeOf<EventLocationViewModel>());
        var vm = result.Model as EventLocationViewModel;
        vm!.CancelLink.Should().Be(AllNetworksUrl);
        vm.PageTitle.Should().Be(Application.Constants.CreateEvent.PageTitle);
    }

    [Test, MoqAutoData]
    public void Details_ReturnsExpectedPostLink(
        [Greedy] NetworkEventLocationController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.ManageEvent.EventLocation, PostUrl);
        var result = (ViewResult)sut.Get();

        Assert.That(result.Model, Is.TypeOf<EventLocationViewModel>());
        var vm = result.Model as EventLocationViewModel;
        vm!.PostLink.Should().Be(PostUrl);
    }

    [TestCase("location 1", null, null)]
    [TestCase("location 2", "event online link", null)]
    [TestCase(null, "event online link", EventFormat.Hybrid)]
    public void Post_SetEventDetailsOnSessionModel(string eventLocation, string? eventOnlineLink, EventFormat? eventFormat)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<EventLocationViewModel>>();

        var sessionModel = new EventSessionModel();

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var submitModel = new EventLocationViewModel { Postcode = eventLocation, OnlineEventLink = eventOnlineLink, EventFormat = eventFormat };

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);

        var sut = new NetworkEventLocationController(sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, AllNetworksUrl);

        var result = (RedirectToRouteResult)sut.Post(submitModel);

        sut.ModelState.IsValid.Should().BeTrue();
        sessionServiceMock.Verify(s => s.Set(It.Is<EventSessionModel>(m => m.EventLocation == eventLocation && m.OnlineEventLink == eventOnlineLink)));
        result.RouteName.Should().Be(submitModel.EventFormat == null
            ? RouteNames.ManageEvent.EventLocation
            : RouteNames.ManageEvent.EventIsAtSchool);
    }

    [Test, MoqAutoData]
    public void Post_WhenNoSelectionOfEventLocation_Errors(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] NetworkEventLocationController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, AllNetworksUrl);

        var sessionModel = new EventSessionModel();

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        sut.ModelState.AddModelError("key", "message");

        var submitModel = new EventLocationViewModel();

        submitModel.EventOnlineLinkMaxLength.Should().Be(1000);

        var result = (ViewResult)sut.Post(submitModel);

        sut.ModelState.IsValid.Should().BeFalse();
        Assert.That(result.Model, Is.TypeOf<EventLocationViewModel>());
        (result.Model as EventLocationViewModel)!.CancelLink.Should().Be(AllNetworksUrl);
    }
}