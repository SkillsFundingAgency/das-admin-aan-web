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
using SFA.DAS.Admin.Aan.Web.Controllers.ManageEvent;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;
using SFA.DAS.Admin.Aan.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers.ManageEvent;
public class EventTypeControllerTests
{
    private static readonly string NetworkEventsUrl = Guid.NewGuid().ToString();
    private static readonly string PostUrl = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void Get_ReturnsCreateEventTypeViewModel(
        [Frozen] Mock<IOuterApiClient> outerApiMock,
        [Frozen] Mock<IValidator<EventTypeViewModel>> validatorMock)
    {
        var sessionServiceMock = new Mock<ISessionService>();

        var sessionModel = new EventSessionModel
        {
            EventTitle = "title",
            CalendarId = 1,
            RegionId = 2,
            EventFormat = EventFormat.Hybrid
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new EventTypeController(outerApiMock.Object, sessionServiceMock.Object, validatorMock.Object);


        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);
        var actualResult = sut.Get(new CancellationToken());
        var viewResult = actualResult.Result.As<ViewResult>();

        outerApiMock.Verify(o => o.GetCalendars(It.IsAny<CancellationToken>()), Times.Once);
        outerApiMock.Verify(o => o.GetRegions(It.IsAny<CancellationToken>()), Times.Once);
        Assert.That(viewResult.Model, Is.TypeOf<EventTypeViewModel>());

        ((EventTypeViewModel)viewResult.Model!).CancelLink.Should().Be(NetworkEventsUrl);
        ((EventTypeViewModel)viewResult.Model!).PageTitle.Should().Be(Application.Constants.CreateEvent.PageTitle);
    }

    [Test, MoqAutoData]
    public void Get_ReturnsExpectedPostLink(
        [Frozen] Mock<IOuterApiClient> outerApiMock,
        [Frozen] Mock<IValidator<EventTypeViewModel>> validatorMock)
    {
        var sessionServiceMock = new Mock<ISessionService>();

        var sessionModel = new EventSessionModel
        {
            EventTitle = "title",
            CalendarId = 1,
            RegionId = 2,
            EventFormat = EventFormat.Hybrid
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new EventTypeController(outerApiMock.Object, sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.ManageEvent.EventType, PostUrl);
        var actualResult = sut.Get(new CancellationToken());
        var viewResult = actualResult.Result.As<ViewResult>();

        ((EventTypeViewModel)viewResult.Model!).PostLink.Should().Be(PostUrl);
    }

    [Test]
    public void Post_SetEventTitleTypeAndRegionOnEmptySessionModel()
    {
        var eventTitle = "title";
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<EventTypeViewModel>>();
        var sessionModel = new EventSessionModel();

        var submitModel = new EventTypeViewModel { EventTitle = eventTitle };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.ValidateAsync(submitModel, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);

        var outerApiMock = new Mock<IOuterApiClient>();
        outerApiMock.Setup(o => o.GetCalendars(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Calendar>());
        outerApiMock.Setup(o => o.GetRegions(It.IsAny<CancellationToken>())).ReturnsAsync(new GetRegionsResult());

        var sut = new EventTypeController(outerApiMock.Object, sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);

        var actualResult = sut.Post(submitModel, new CancellationToken());

        var result = actualResult.Result.As<RedirectToRouteResult>();

        sut.ModelState.IsValid.Should().BeTrue();
        sessionServiceMock.Verify(s => s.Set(It.Is<EventSessionModel>(m => m.EventTitle == eventTitle)));
        result.RouteName.Should().Be(RouteNames.ManageEvent.Description);
    }

    [Test, MoqAutoData]
    public void Post_WhenValidationErrors_RedirectToEventFormat(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] EventTypeController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);

        var sessionModel = new EventSessionModel();
        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        sut.ModelState.AddModelError("key", "message");

        var submitModel = new EventTypeViewModel();
        var actualResult = sut.Post(submitModel, new CancellationToken());

        var result = actualResult.Result.As<ViewResult>();

        sut.ModelState.IsValid.Should().BeFalse();
        Assert.That(result.Model, Is.TypeOf<EventTypeViewModel>());
        (result.Model as EventTypeViewModel)!.CancelLink.Should().Be(NetworkEventsUrl);
        sessionServiceMock.Verify(s => s.Set(It.IsAny<EventSessionModel>()), Times.Never());
    }
}
