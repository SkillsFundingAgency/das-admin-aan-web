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

public class NetworkEventOrganiserNameControllerTests
{
    private static readonly string AllNetworksUrl = Guid.NewGuid().ToString();
    private static readonly string PostUrl = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void Details_ReturnsCreateEventDetailsViewModel(
        [Greedy] NetworkEventOrganiserNameController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, AllNetworksUrl);
        var result = (ViewResult)sut.Get();

        Assert.That(result.Model, Is.TypeOf<EventOrganiserNameViewModel>());
        var vm = result.Model as EventOrganiserNameViewModel;
        vm!.CancelLink.Should().Be(AllNetworksUrl);
        vm.PageTitle.Should().Be(CreateEvent.PageTitle);
    }

    [Test, MoqAutoData]
    public void Details_ReturnsExpectedPostLink(
        [Greedy] NetworkEventOrganiserNameController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.ManageEvent.EventOrganiserName, PostUrl);
        var result = (ViewResult)sut.Get();

        Assert.That(result.Model, Is.TypeOf<EventOrganiserNameViewModel>());
        var vm = result.Model as EventOrganiserNameViewModel;
        vm!.PostLink.Should().Be(PostUrl);
    }

    [TestCase("location 1", null)]
    [TestCase("location 2", "event online link")]
    [TestCase(null, "event online link")]
    public void Post_SetEventDetailsOnSessionModel(string organiserName, string? organiserEmail)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<EventOrganiserNameViewModel>>();

        var sessionModel = new EventSessionModel();

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var submitModel = new EventOrganiserNameViewModel { OrganiserName = organiserName, OrganiserEmail = organiserEmail };

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);

        var sut = new NetworkEventOrganiserNameController(sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, AllNetworksUrl);

        var result = (RedirectToRouteResult)sut.Post(submitModel);

        sut.ModelState.IsValid.Should().BeTrue();
        sessionServiceMock.Verify(s => s.Set(It.Is<EventSessionModel>(m => m.OrganiserName == organiserName && m.OrganiserEmail == organiserEmail)));
        result.RouteName.Should().Be(RouteNames.ManageEvent.EventOrganiserName);
    }

    [Test, MoqAutoData]
    public void Post_WhenNoSelectionOfEventLocation_Errors(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] NetworkEventOrganiserNameController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, AllNetworksUrl);

        var sessionModel = new EventSessionModel();

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        sut.ModelState.AddModelError("key", "message");

        var submitModel = new EventOrganiserNameViewModel();
        var result = (ViewResult)sut.Post(submitModel);

        sut.ModelState.IsValid.Should().BeFalse();
        Assert.That(result.Model, Is.TypeOf<EventOrganiserNameViewModel>());
        (result.Model as EventOrganiserNameViewModel)!.CancelLink.Should().Be(AllNetworksUrl);
    }
}