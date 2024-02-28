using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Web.Controllers.ManageEvent;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;
using SFA.DAS.Admin.Aan.Web.UnitTests.TestHelpers;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers.ManageEvent;
public class EventPublishedControllerTests
{
    private static readonly string NetworkEventsUrl = Guid.NewGuid().ToString();

    [Test]
    public void GetPublishEvent()
    {
        var sut = new EventPublishedController();
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);
        var result = sut.Get();
        var actualResult = result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<EventPublishedViewModel>());
        var vm = actualResult.Model as EventPublishedViewModel;
        vm!.ManageEventsLink.Should().Be(NetworkEventsUrl);
    }
}
