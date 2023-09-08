using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Admin.Aan.Application.Constants;
using SFA.DAS.Admin.Aan.Application.OuterApi.Calendar;
using SFA.DAS.Admin.Aan.Application.OuterApi.Regions;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Controllers.CreateEvent;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;
using SFA.DAS.Admin.Aan.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers;
public class NetworkEventTypeControllerTests
{
    private static readonly string NetworkEventsUrl = Guid.NewGuid().ToString();
    private static readonly string PostUrl = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void Get_ReturnsCreateEventTypeViewModel(
        [Frozen] Mock<IOuterApiClient> outerApiMock,
        [Frozen] Mock<IValidator<CreateEventTypeViewModel>> validatorMock)
    {
        var sessionServiceMock = new Mock<ISessionService>();

        var sessionModel = new CreateEventSessionModel
        {
            EventTitle = "title",
            EventTypeId = 1,
            EventRegionId = 2,
            EventFormat = EventFormat.Hybrid
        };

        sessionServiceMock.Setup(s => s.Get<CreateEventSessionModel>()).Returns(sessionModel);

        var sut = new NetworkEventTypeController(outerApiMock.Object, sessionServiceMock.Object, validatorMock.Object);


        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);
        var actualResult = sut.Get(new CancellationToken());
        var viewResult = actualResult.Result.As<ViewResult>();

        outerApiMock.Verify(o => o.GetCalendars(It.IsAny<CancellationToken>()), Times.Once);
        outerApiMock.Verify(o => o.GetRegions(It.IsAny<CancellationToken>()), Times.Once);
        Assert.That(viewResult.Model, Is.TypeOf<CreateEventTypeViewModel>());

        ((CreateEventTypeViewModel)viewResult.Model!).CancelLink.Should().Be(NetworkEventsUrl);
        ((CreateEventTypeViewModel)viewResult.Model!).PageTitle.Should().Be(CreateEvent.PageTitle);
    }

    [Test, MoqAutoData]
    public void Get_ReturnsExpectedPostLink(
        [Frozen] Mock<IOuterApiClient> outerApiMock,
        [Frozen] Mock<IValidator<CreateEventTypeViewModel>> validatorMock)
    {
        var sessionServiceMock = new Mock<ISessionService>();

        var sessionModel = new CreateEventSessionModel
        {
            EventTitle = "title",
            EventTypeId = 1,
            EventRegionId = 2,
            EventFormat = EventFormat.Hybrid
        };

        sessionServiceMock.Setup(s => s.Get<CreateEventSessionModel>()).Returns(sessionModel);

        var sut = new NetworkEventTypeController(outerApiMock.Object, sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.EventType, PostUrl);
        var actualResult = sut.Get(new CancellationToken());
        var viewResult = actualResult.Result.As<ViewResult>();

        ((CreateEventTypeViewModel)viewResult.Model!).PostLink.Should().Be(PostUrl);
    }

    [Test]
    public void Post_SetEventTitleTypeAndRegionOnEmptySessionModel()
    {
        var eventTitle = "title";
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<CreateEventTypeViewModel>>();
        var sessionModel = new CreateEventSessionModel();

        var submitModel = new CreateEventTypeViewModel { EventTitle = eventTitle };

        sessionServiceMock.Setup(s => s.Get<CreateEventSessionModel>()).Returns(sessionModel);

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.ValidateAsync(submitModel, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);

        var outerApiMock = new Mock<IOuterApiClient>();
        outerApiMock.Setup(o => o.GetCalendars(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Calendar>());
        outerApiMock.Setup(o => o.GetRegions(It.IsAny<CancellationToken>())).ReturnsAsync(new GetRegionsResult());

        var sut = new NetworkEventTypeController(outerApiMock.Object, sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);

        var actualResult = sut.Post(submitModel, new CancellationToken());

        var result = actualResult.Result.As<RedirectToActionResult>();

        sut.ModelState.IsValid.Should().BeTrue();
        sessionServiceMock.Verify(s => s.Set(It.Is<CreateEventSessionModel>(m => m.EventTitle == eventTitle)));
        result.ControllerName.Should().Be("NetworkEventDescription");
        result.ActionName.Should().Be("Get");
    }

    [Test]
    public void Post_SetEventTypeOnNoSessionModel()
    {
        var validatorMock = new Mock<IValidator<CreateEventTypeViewModel>>();
        var sessionServiceMock = new Mock<ISessionService>();
        sessionServiceMock.Setup(s => s.Get<CreateEventSessionModel>()).Returns((CreateEventSessionModel)null!);

        var submitModel = new CreateEventTypeViewModel();

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.ValidateAsync(submitModel, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);

        var sut = new NetworkEventTypeController(Mock.Of<IOuterApiClient>(), sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);

        var actualResult = sut.Post(submitModel, new CancellationToken());
        var result = actualResult.Result.As<RedirectToActionResult>();
        result.ControllerName.Should().Be("NetworkEventFormat");
        result.ActionName.Should().Be("Get");
    }

    [Test, MoqAutoData]
    public void Post_WhenValidationErrors_RedirectToEventFormat(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] NetworkEventTypeController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);

        var sessionModel = new CreateEventSessionModel();
        sessionServiceMock.Setup(s => s.Get<CreateEventSessionModel>()).Returns(sessionModel);

        sut.ModelState.AddModelError("key", "message");

        var submitModel = new CreateEventTypeViewModel();
        var actualResult = sut.Post(submitModel, new CancellationToken());

        var result = actualResult.Result.As<ViewResult>();

        sut.ModelState.IsValid.Should().BeFalse();
        Assert.That(result.Model, Is.TypeOf<CreateEventTypeViewModel>());
        (result.Model as CreateEventTypeViewModel)!.CancelLink.Should().Be(NetworkEventsUrl);
    }
}
