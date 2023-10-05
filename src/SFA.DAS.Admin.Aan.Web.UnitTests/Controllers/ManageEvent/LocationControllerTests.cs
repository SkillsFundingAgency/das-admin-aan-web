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

public class LocationControllerTests
{
    private static readonly string AllNetworksUrl = Guid.NewGuid().ToString();
    private static readonly string PostUrl = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void Get_ReturnsLocationViewModel(
        [Greedy] LocationController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, AllNetworksUrl);
        var result = (ViewResult)sut.Get();

        Assert.That(result.Model, Is.TypeOf<LocationViewModel>());
        var vm = result.Model as LocationViewModel;
        vm!.CancelLink.Should().Be(AllNetworksUrl);
        vm.PageTitle.Should().Be(Application.Constants.CreateEvent.PageTitle);
    }

    [Test, MoqAutoData]
    public void Get_ReturnsExpectedPostLink(
        [Greedy] LocationController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.ManageEvent.Location, PostUrl);
        var result = (ViewResult)sut.Get();

        Assert.That(result.Model, Is.TypeOf<LocationViewModel>());
        var vm = result.Model as LocationViewModel;
        vm!.PostLink.Should().Be(PostUrl);
    }

    [TestCase("location 1", null, null)]
    [TestCase("location 2", "event online link", null)]
    [TestCase(null, "event online link", EventFormat.Hybrid)]
    public void Post_SetEventDetailsOnSessionModel(string eventLocation, string? eventOnlineLink, EventFormat? eventFormat)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<LocationViewModel>>();

        var sessionModel = new EventSessionModel();

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var submitModel = new LocationViewModel { Postcode = eventLocation, OnlineEventLink = eventOnlineLink, EventFormat = eventFormat };

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);

        var sut = new LocationController(sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, AllNetworksUrl);

        var result = (RedirectToRouteResult)sut.Post(submitModel);

        sut.ModelState.IsValid.Should().BeTrue();
        sessionServiceMock.Verify(s => s.Set(It.Is<EventSessionModel>(m => m.Location == eventLocation && m.EventLink == eventOnlineLink)));
        result.RouteName.Should().Be(submitModel.EventFormat == null
            ? RouteNames.ManageEvent.OrganiserDetails
            : RouteNames.ManageEvent.IsAtSchool);
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
}