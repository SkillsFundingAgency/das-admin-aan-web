using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Admin.Aan.Application.OuterApi.CalendarEvents;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Controllers.ManageEvent;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;
using SFA.DAS.Admin.Aan.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers;

[TestFixture]
public class NotifyAttendeesControllerTests
{
    private static readonly string NetworkEventsUrl = Guid.NewGuid().ToString();
    private static readonly string NotifyAttendeesUrl = Guid.NewGuid().ToString();
    private static readonly string UpdateEventConfirmationUrl = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void Get_ReturnsNotifyAttendeesViewModel(
        [Greedy] NotifyAttendeesController sut)
    {
        var calendarEventId = Guid.NewGuid();

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.UpdateEvent.NotifyAttendees, NotifyAttendeesUrl);
        var result = (ViewResult)sut.Get(calendarEventId);

        Assert.That(result.Model, Is.TypeOf<NotifyAttendeesViewModel>());
        var vm = result.Model as NotifyAttendeesViewModel;
        vm!.PostLink.Should().Be(NotifyAttendeesUrl);
    }

    [Test, MoqAutoData]
    public void Get_ReturnsExpectedCancelLink(
        [Greedy] NotifyAttendeesController sut)
    {
        var calendarEventId = Guid.NewGuid();

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);
        var result = (ViewResult)sut.Get(calendarEventId);

        Assert.That(result.Model, Is.TypeOf<NotifyAttendeesViewModel>());
        var vm = result.Model as NotifyAttendeesViewModel;
        vm!.CancelLink.Should().Be(NetworkEventsUrl);
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task Post_UpdateCalendarEventAndRedirectToConfirmationPage(bool isNotifyAttendees)
    {
        var calendarEventId = Guid.NewGuid();
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<NotifyAttendeesViewModel>>();

        var sessionModel = new EventSessionModel { CalendarEventId = calendarEventId };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var submitModel = new NotifyAttendeesViewModel { IsNotifyAttendees = isNotifyAttendees };

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.ValidateAsync(submitModel, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        var outerApiMock = new Mock<IOuterApiClient>();

        var sut = new NotifyAttendeesController(outerApiMock.Object, sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.UpdateEventConfirmation, UpdateEventConfirmationUrl);

        var actualResult = await sut.Post(submitModel, calendarEventId, new CancellationToken());

        var result = (RedirectToRouteResult)actualResult;

        sut.ModelState.IsValid.Should().BeTrue();

        outerApiMock.Verify(
            o => o.UpdateCalendarEvent(It.IsAny<Guid>(), calendarEventId,
                It.Is<UpdateCalendarEventRequest>(r => r.SendUpdateEventNotification == isNotifyAttendees),
                It.IsAny<CancellationToken>()), Times.Once);
        result.RouteName.Should().Be(RouteNames.UpdateEventConfirmation);
    }

    [Test, MoqAutoData]
    public async Task Post_WhenNoSelectionOfNotifyAttendees_Errors(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] NotifyAttendeesController sut)
    {
        var calendarEventId = Guid.NewGuid();
        sut.ModelState.AddModelError("key", "message");

        var submitModel = new NotifyAttendeesViewModel { CancelLink = NetworkEventsUrl };

        var actualResult = await sut.Post(submitModel, calendarEventId, new CancellationToken());
        var result = (ViewResult)actualResult;

        sut.ModelState.IsValid.Should().BeFalse();
        Assert.That(result.Model, Is.TypeOf<NotifyAttendeesViewModel>());
        (result.Model as NotifyAttendeesViewModel)!.CancelLink.Should().Be(NetworkEventsUrl);
    }
}