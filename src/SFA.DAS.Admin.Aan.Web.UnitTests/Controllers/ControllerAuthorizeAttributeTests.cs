using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Web.Authentication;
using SFA.DAS.Admin.Aan.Web.Controllers;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers;

public class ControllerAuthorizeAttributeTests
{
    private readonly List<string> _controllersThatDoNotRequireAuthorize =
    [nameof(AccountController)];

    [Test]
    public void Controllers_MustBeDecoratedWithAuthorizeAttribute()
    {
        var webAssembly = typeof(HomeController).Assembly;

        var controllers = webAssembly.DefinedTypes.Where(c => c.IsSubclassOf(typeof(ControllerBase))).ToList();

        using (new AssertionScope())
        {
            foreach (var controller in controllers)
            {
                if (!_controllersThatDoNotRequireAuthorize.Contains(controller.Name))
                {
                    controller.Should().BeDecoratedWith<AuthorizeAttribute>();
                }

                if (controller.FullName!.Contains("ManageEvent"))
                {
                    controller.Should().BeDecoratedWith<AuthorizeAttribute>(attr => attr.Roles!.Contains(Roles.ManageEventsRole));
                }

                if (controller.FullName!.Contains("ManageMembers"))
                {
                    controller.Should().BeDecoratedWith<AuthorizeAttribute>(attr => attr.Roles!.Contains(Roles.ManageMembersRole));
                }
            }
        }
    }

}
