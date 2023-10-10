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
public class EventDescriptionControllerTests
{
    private static readonly string AllNetworksUrl = Guid.NewGuid().ToString();
    private static readonly string PostUrl = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void Get_ReturnsDescriptionViewModel(
        [Greedy] EventDescriptionController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, AllNetworksUrl);
        var result = (ViewResult)sut.Get();

        Assert.That(result.Model, Is.TypeOf<EventDescriptionViewModel>());
        var vm = result.Model as EventDescriptionViewModel;
        vm!.CancelLink.Should().Be(AllNetworksUrl);
        vm.PageTitle.Should().Be(Application.Constants.CreateEvent.PageTitle);
    }

    [Test, MoqAutoData]
    public void Get_ReturnsExpectedPostLink(
        [Greedy] EventDescriptionController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.ManageEvent.EventFormat, PostUrl);
        var result = (ViewResult)sut.Get();

        Assert.That(result.Model, Is.TypeOf<EventDescriptionViewModel>());
        var vm = result.Model as EventDescriptionViewModel;
        vm!.PostLink.Should().Be(PostUrl);
    }

    [TestCase("outline 1 ", " summary 1")]
    [TestCase(" outline 2", "summary 2 ")]
    public void Post_SetEventDetailsOnSessionModel(string eventOutline, string eventSummary)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<EventDescriptionViewModel>>();

        var sessionModel = new EventSessionModel();

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var submitModel = new EventDescriptionViewModel { EventOutline = eventOutline, EventSummary = eventSummary };

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);

        var sut = new EventDescriptionController(sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, AllNetworksUrl);

        var result = (RedirectToRouteResult)sut.Post(submitModel);

        sut.ModelState.IsValid.Should().BeTrue();
        sessionServiceMock.Verify(s => s.Set(It.Is<EventSessionModel>(m => m.EventOutline == eventOutline.Trim() && m.EventSummary == eventSummary.Trim())));
        result.RouteName.Should().Be(RouteNames.ManageEvent.HasGuestSpeakers);
    }

    [Test, MoqAutoData]
    public void Post_WhenNoSelectionOfEventDescription_Errors(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] EventDescriptionController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, AllNetworksUrl);

        sut.ModelState.AddModelError("key", "message");

        var submitModel = new EventDescriptionViewModel { CancelLink = AllNetworksUrl };

        submitModel.EventOutlineMaxCount.Should().Be(ManageEventValidation.EventOutlineMaxLength);
        submitModel.EventSummaryMaxCount.Should().Be(ManageEventValidation.EventSummaryMaxLength);

        var result = (ViewResult)sut.Post(submitModel);

        sut.ModelState.IsValid.Should().BeFalse();
        Assert.That(result.Model, Is.TypeOf<EventDescriptionViewModel>());
        (result.Model as EventDescriptionViewModel)!.CancelLink.Should().Be(AllNetworksUrl);
        sessionServiceMock.Verify(s => s.Set(It.IsAny<EventSessionModel>()), Times.Never());
    }
}