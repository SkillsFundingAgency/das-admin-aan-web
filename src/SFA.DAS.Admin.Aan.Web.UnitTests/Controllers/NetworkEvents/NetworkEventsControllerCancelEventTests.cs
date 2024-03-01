using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Admin.Aan.Application.OuterApi.Calendar.Responses;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Controllers;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.DeleteEvent;
using SFA.DAS.Admin.Aan.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers.NetworkEvents;
public class NetworkEventsControllerCancelEventTests
{
    private static readonly string NetworkEventsUrl = Guid.NewGuid().ToString();
    private static readonly string DeleteEventUrl = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void Get_ReturnsCancelEventViewModel(
        [Frozen] Mock<IOuterApiClient> outerApiMock,
        [Frozen] Mock<IValidator<CancelEventViewModel>> validatorMock,
        GetCalendarEventQueryResult result,
        Guid calendarEventId,
        Guid memberId)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        result.CalendarEventId = calendarEventId;
        sessionServiceMock.Setup(s => s.GetMemberId()).Returns(memberId);
        outerApiMock.Setup(o => o.GetCalendarEvent(memberId, calendarEventId, It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var sut = new NetworkEventsController(outerApiMock.Object, sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.DeleteEvent, DeleteEventUrl);

        var actualResult = sut.CancelEvent(calendarEventId, new CancellationToken());
        var viewResult = actualResult.Result.As<ViewResult>();

        outerApiMock.Verify(o => o.GetCalendarEvent(memberId, calendarEventId, It.IsAny<CancellationToken>()), Times.Once);
        Assert.That(viewResult.Model, Is.TypeOf<CancelEventViewModel>());

        ((CancelEventViewModel)viewResult.Model!).PostLink.Should().Be(DeleteEventUrl);
    }

    [Test, MoqAutoData]
    public void Get_ReturnsCancelEventViewModel_WithNetworkEventsUrlSet(
        [Frozen] Mock<IOuterApiClient> outerApiMock,
        [Frozen] Mock<IValidator<CancelEventViewModel>> validatorMock,
        GetCalendarEventQueryResult result,
        Guid calendarEventId,
        Guid memberId)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        result.CalendarEventId = calendarEventId;
        sessionServiceMock.Setup(s => s.GetMemberId()).Returns(memberId);
        outerApiMock.Setup(o => o.GetCalendarEvent(memberId, calendarEventId, It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var sut = new NetworkEventsController(outerApiMock.Object, sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);

        var actualResult = sut.CancelEvent(calendarEventId, new CancellationToken());
        var viewResult = actualResult.Result.As<ViewResult>();

        outerApiMock.Verify(o => o.GetCalendarEvent(memberId, calendarEventId, It.IsAny<CancellationToken>()), Times.Once);
        Assert.That(viewResult.Model, Is.TypeOf<CancelEventViewModel>());

        ((CancelEventViewModel)viewResult.Model!).ManageEventsLink.Should().Be(NetworkEventsUrl);
    }

    [Test, MoqAutoData]
    public void Post_IsCancelConfirmed_True_RedirectsToCancelEventConfirmation(
        [Frozen] Mock<IOuterApiClient> outerApiMock,
        [Frozen] Mock<IValidator<CancelEventViewModel>> validatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        Guid calendarEventId,
        Guid memberId,
        string title)
    {
        sessionServiceMock.Setup(s => s.GetMemberId()).Returns(memberId);

        var submitModel = new CancelEventViewModel
        {
            CalendarEventId = calendarEventId,
            Title = title,
            PostLink = DeleteEventUrl,
            ManageEventsLink = NetworkEventsUrl,
            IsCancelConfirmed = true
        };

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);

        var sut = new NetworkEventsController(outerApiMock.Object, sessionServiceMock.Object, validatorMock.Object);

        var actualResult = sut.PostCancelEvent(submitModel, new CancellationToken());
        var result = actualResult.Result.As<RedirectToRouteResult>();
        result.RouteName.Should().Be(RouteNames.DeleteEventConfirmation);
        outerApiMock.Verify(o => o.DeleteCalendarEvent(memberId, calendarEventId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test, MoqAutoData]
    public void Post_IsCancelConfirmed_False_RedirectsBackToCancelEventPage(
        [Frozen] Mock<IOuterApiClient> outerApiMock,
        [Frozen] Mock<IValidator<CancelEventViewModel>> validatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        Guid calendarEventId,
        Guid memberId,
        string title)
    {
        sessionServiceMock.Setup(s => s.GetMemberId()).Returns(memberId);

        var submitModel = new CancelEventViewModel
        {
            CalendarEventId = calendarEventId,
            Title = title,
            PostLink = DeleteEventUrl,
            ManageEventsLink = NetworkEventsUrl,
            IsCancelConfirmed = false
        };

        var sut = new NetworkEventsController(outerApiMock.Object, sessionServiceMock.Object, validatorMock.Object);
        sut.ModelState.AddModelError("key", "message");
        sut.ModelState.IsValid.Should().BeFalse();
        var actualResult = sut.PostCancelEvent(submitModel, new CancellationToken());
        var viewResult = actualResult.Result.As<ViewResult>();

        Assert.That(viewResult.Model, Is.TypeOf<CancelEventViewModel>());
        outerApiMock.Verify(o => o.DeleteCalendarEvent(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public void GetDeleteEventConfirmation_ReturnsDeleteEventConfirmationViewModel()
    {
        var sut = new NetworkEventsController(Mock.Of<IOuterApiClient>(), Mock.Of<ISessionService>(), Mock.Of<IValidator<CancelEventViewModel>>());
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);
        var result = sut.DeleteEventConfirmation();
        var viewResult = result.As<ViewResult>();

        Assert.That(viewResult.Model, Is.TypeOf<CancelEventConfirmationViewModel>());

        ((CancelEventConfirmationViewModel)viewResult.Model!).ManageEventsLink.Should().Be(NetworkEventsUrl);
    }
}
