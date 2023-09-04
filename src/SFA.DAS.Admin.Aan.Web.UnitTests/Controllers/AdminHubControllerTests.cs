using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Web.Controllers;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models;
using SFA.DAS.Admin.Aan.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers;

public class AdminHubControllerTests
{
    private static readonly string AllNetworksUrl = Guid.NewGuid().ToString();
    [Test, MoqAutoData]
    public void GetAdminHub_GeneratesExpectedViewModel(
        [Greedy] AdminHubController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, AllNetworksUrl);

        var actualResult = sut.Index();
        var viewResult = actualResult.As<ViewResult>();
        var model = viewResult.Model as AdminHubViewModel;
        model!.ManageEventsUrl.Should().Be(AllNetworksUrl);
    }
}
