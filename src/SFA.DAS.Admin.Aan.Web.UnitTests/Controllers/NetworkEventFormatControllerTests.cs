using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Admin.Aan.Application.Constants;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Controllers;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;
using SFA.DAS.Admin.Aan.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers;
public class NetworkEventFormatControllerTests
{
    private static readonly string AllNetworksUrl = Guid.NewGuid().ToString();

    [Test]
    [MoqAutoData]
    public void Details_ReturnsCreateEventFormatViewModel(
        [Greedy] NetworkEventFormatController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, AllNetworksUrl);
        var result = (ViewResult)sut.Get();

        Assert.That(result.Model, Is.TypeOf<CreateEventFormatViewModel>());
        var vm = result.Model as CreateEventFormatViewModel;
        vm!.BackLink.Should().Be(AllNetworksUrl);
    }

    [TestCase(EventFormat.InPerson)]
    [TestCase(EventFormat.Hybrid)]
    [TestCase(EventFormat.Online)]
    public void Post_SetNullEventFormatOnSessionModel(EventFormat eventFormat)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<CreateEventFormatViewModel>>();

        var sessionModel = new CreateEventSessionModel();

        sessionServiceMock.Setup(s => s.Get<CreateEventSessionModel>()).Returns(sessionModel);

        var submitModel = new CreateEventFormatViewModel { EventFormat = eventFormat };

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);

        var sut = new NetworkEventFormatController(sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, AllNetworksUrl);

        var result = (ViewResult)sut.Post(submitModel);

        sut.ModelState.IsValid.Should().BeTrue();
        sessionServiceMock.Verify(s => s.Set(It.Is<CreateEventSessionModel>(m => m.EventFormat == eventFormat)));

        Assert.That(result.Model, Is.TypeOf<CreateEventFormatViewModel>());
        var vm = result.Model as CreateEventFormatViewModel;
        vm!.BackLink.Should().Be(AllNetworksUrl);
    }

    [MoqAutoData]
    public void Post_WhenNoSelectionOfEventFormat_Errors(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] NetworkEventFormatController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, AllNetworksUrl);


        var sessionModel = new CreateEventSessionModel();

        sessionServiceMock.Setup(s => s.Get<CreateEventSessionModel>()).Returns(sessionModel);

        sut.ModelState.AddModelError("key", "message");

        var submitModel = new CreateEventFormatViewModel();

        var result = (ViewResult)sut.Post(submitModel);

        sut.ModelState.IsValid.Should().BeFalse();
        Assert.That(result.Model, Is.TypeOf<CreateEventFormatViewModel>());
        var vm = result.Model as CreateEventFormatViewModel;
        vm!.BackLink.Should().Be(AllNetworksUrl);
    }
}
