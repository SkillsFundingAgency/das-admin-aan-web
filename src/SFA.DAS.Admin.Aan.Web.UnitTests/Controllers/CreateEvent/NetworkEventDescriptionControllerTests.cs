using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Controllers.CreateEvent;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;
using SFA.DAS.Admin.Aan.Web.UnitTests.TestHelpers;
using SFA.DAS.Admin.Aan.Web.Validators.CreateEvent;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers.CreateEvent;
public class NetworkEventDescriptionControllerTests
{
    private static readonly string AllNetworksUrl = Guid.NewGuid().ToString();
    private static readonly string PostUrl = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void Details_ReturnsCreateEventDetailsViewModel(
        [Greedy] NetworkEventDescriptionController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, AllNetworksUrl);
        var result = (ViewResult)sut.Get();

        Assert.That(result.Model, Is.TypeOf<CreateEventDescriptionViewModel>());
        var vm = result.Model as CreateEventDescriptionViewModel;
        vm!.CancelLink.Should().Be(AllNetworksUrl);
        vm.PageTitle.Should().Be(Application.Constants.CreateEvent.PageTitle);
    }

    [Test, MoqAutoData]
    public void Details_ReturnsExpectedPostLink(
        [Greedy] NetworkEventDescriptionController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.EventFormat, PostUrl);
        var result = (ViewResult)sut.Get();

        Assert.That(result.Model, Is.TypeOf<CreateEventDescriptionViewModel>());
        var vm = result.Model as CreateEventDescriptionViewModel;
        vm!.PostLink.Should().Be(PostUrl);
    }

    [TestCase("outline 1", "summary 1")]
    [TestCase("outline 2", "summary 2")]
    public void Post_SetEventDetailsOnSessionModel(string eventOutline, string eventSummary)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<CreateEventDescriptionViewModel>>();

        var sessionModel = new CreateEventSessionModel();

        sessionServiceMock.Setup(s => s.Get<CreateEventSessionModel>()).Returns(sessionModel);

        var submitModel = new CreateEventDescriptionViewModel { EventOutline = eventOutline, EventSummary = eventSummary };

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);

        var sut = new NetworkEventDescriptionController(sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, AllNetworksUrl);

        var result = (RedirectToRouteResult)sut.Post(submitModel);

        sut.ModelState.IsValid.Should().BeTrue();
        sessionServiceMock.Verify(s => s.Set(It.Is<CreateEventSessionModel>(m => m.EventOutline == eventOutline && m.EventSummary == eventSummary)));
        result.RouteName.Should().Be(RouteNames.CreateEvent.EventHasGuestSpeakers);
    }

    [Test, MoqAutoData]
    public void Post_WhenNoSelectionOfEventDetails_Errors(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] NetworkEventDescriptionController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, AllNetworksUrl);

        var sessionModel = new CreateEventSessionModel();

        sessionServiceMock.Setup(s => s.Get<CreateEventSessionModel>()).Returns(sessionModel);

        sut.ModelState.AddModelError("key", "message");

        var submitModel = new CreateEventDescriptionViewModel();

        submitModel.EventOutlineMaxCount.Should().Be(CreateEventDescriptionViewModelValidator.EventOutlineMaxLength);
        submitModel.EventSummaryMaxCount.Should().Be(CreateEventDescriptionViewModelValidator.EventSummaryMaxLength);

        var result = (ViewResult)sut.Post(submitModel);

        sut.ModelState.IsValid.Should().BeFalse();
        Assert.That(result.Model, Is.TypeOf<CreateEventDescriptionViewModel>());
        (result.Model as CreateEventDescriptionViewModel)!.CancelLink.Should().Be(AllNetworksUrl);
    }
}