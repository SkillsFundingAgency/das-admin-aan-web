using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Controllers.ManageEvent;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;
using SFA.DAS.Admin.Aan.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers.ManageEvent;
public class EventPublishedControllerTests
{
    private static readonly string NetworkEventsUrl = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void GetPublishEvent(Guid eventId)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var sut = new EventPublishedController();
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);
        var result = sut.Get(eventId);
        var actualResult = result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<EventPublishedViewModel>());
        var vm = actualResult.Model as EventPublishedViewModel;
        vm!.ManageEventsLink.Should().Be(NetworkEventsUrl);
    }
}
