using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
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
}
