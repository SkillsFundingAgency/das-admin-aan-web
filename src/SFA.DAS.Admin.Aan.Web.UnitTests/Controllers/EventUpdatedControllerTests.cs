using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Web.Controllers.ManageEvent;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;
using SFA.DAS.Admin.Aan.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers;
public class EventUpdatedControllerTests
{
    private static readonly string NetworkEventsUrl = Guid.NewGuid().ToString();
    private static readonly string AdministratorHubLink = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void Get_ReturnsEventUpdatedViewModel_ManageEventsLink(
        [Greedy] EventUpdatedController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);
        var result = (ViewResult)sut.Get(Guid.NewGuid());

        Assert.That(result.Model, Is.TypeOf<EventUpdatedViewModel>());
        var vm = result.Model as EventUpdatedViewModel;
        vm!.ManageEventsLink.Should().Be(NetworkEventsUrl);
    }

    [Test, MoqAutoData]
    public void Get_ReturnsEventUpdatedViewModel_AdministratorHubLink(
        [Greedy] EventUpdatedController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AdministratorHub, AdministratorHubLink);
        var result = (ViewResult)sut.Get(Guid.NewGuid());

        Assert.That(result.Model, Is.TypeOf<EventUpdatedViewModel>());
        var vm = result.Model as EventUpdatedViewModel;
        vm!.AdministratorHubLink.Should().Be(AdministratorHubLink);
    }
}
