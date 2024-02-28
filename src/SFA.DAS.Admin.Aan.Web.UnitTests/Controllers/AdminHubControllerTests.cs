using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Aan.SharedUi.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Authentication;
using SFA.DAS.Admin.Aan.Web.Controllers;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models;
using SFA.DAS.Admin.Aan.Web.UnitTests.TestHelpers;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers;

public class AdminHubControllerTests
{
    private static readonly string AllNetworksUrl = Guid.NewGuid().ToString();
    private static readonly string NetworkDirectoryUrl = Guid.NewGuid().ToString();
    private AdminHubViewModel _viewModel = null!;

    [SetUp]
    public void GivenUserHasAllRoles()
    {
        AdminHubController sut = new();
        sut
            .AddControllerContextWithAllRoles()
            .AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.NetworkEvents, AllNetworksUrl)
            .AddUrlForRoute(SharedRouteNames.NetworkDirectory, NetworkDirectoryUrl);

        //action
        var actualResult = sut.Index();
        _viewModel = actualResult.As<ViewResult>().Model.As<AdminHubViewModel>();
    }

    [Test]
    public void GetAdminHub_HasManageEventsUrl()
    {
        _viewModel.ManageEventsUrl.Should().Be(AllNetworksUrl);
    }

    [Test]
    public void GetAdminHub_HasManageAmbassadorsUrl()
    {
        _viewModel.ManageAmbassadorsUrl.Should().Be(NetworkDirectoryUrl);
    }

    [Test]
    public void GetAdminHub_HasManageEventsRole()
    {
        _viewModel.HasManageEventsRole.Should().BeTrue();
    }

    [Test]
    public void GetAdminHub_HasManageMembersRole()
    {
        _viewModel.HasManageMembersRole.Should().BeTrue();
    }
}

public class AdminHubControllerRoleTests
{
    [TestCase(Roles.ManageEventsRole, true, false)]
    [TestCase(Roles.ManageMembersRole, false, true)]
    public void GetAdminHub_HasCorrectRoleFlagsSet(string role, bool hasManageEvents, bool hasManageAmbassadors)
    {
        AdminHubController sut = new();
        sut
            .AddControllerContextWithRoles(new[] { role })
            .AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.NetworkEvents, TestConstants.DefaultUrl);

        //action
        var actualResult = sut.Index();
        var viewModel = actualResult.As<ViewResult>().Model.As<AdminHubViewModel>();

        viewModel.HasManageEventsRole.Should().Be(hasManageEvents);
        viewModel.HasManageMembersRole.Should().Be(hasManageAmbassadors);
    }
}
