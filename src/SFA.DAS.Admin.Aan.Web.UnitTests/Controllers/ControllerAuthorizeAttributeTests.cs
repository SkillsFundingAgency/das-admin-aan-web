using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Web.Authentication;
using SFA.DAS.Admin.Aan.Web.Controllers;
using System.Reflection;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers;

public class ControllerAuthorizeAttributeTests
{
    private readonly List<string> _controllersThatDoNotRequireAuthorize = new List<string>()
    {
        nameof(AccountController)
    };

    [Test]
    public void Controllers_MustBeDecoratedWithAuthorizeAttribute()
    {
        var webAssembly = typeof(HomeController).GetTypeInfo().Assembly;

        var controllers = webAssembly.DefinedTypes.Where(c => c.IsSubclassOf(typeof(ControllerBase))).ToList();

        using (new AssertionScope())
        {
            foreach (var controller in controllers.Where(c => !_controllersThatDoNotRequireAuthorize.Contains(c.Name)))
            {
                controller.Should().BeDecoratedWith<AuthorizeAttribute>();
            }

            foreach (var controller in controllers.Where(c => c.FullName!.Contains("Event")))
            {
                controller.Should().BeDecoratedWith<AuthorizeAttribute>(attr => attr.Roles!.Contains(Roles.ManageEventsRole));
            }
        }
    }

}
