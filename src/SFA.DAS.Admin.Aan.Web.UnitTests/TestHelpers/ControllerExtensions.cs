using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using SFA.DAS.Admin.Aan.Web.Authentication;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.TestHelpers;
public static class ControllerExtensions
{
    public static Mock<IUrlHelper> AddUrlHelperMock(this Controller controller)
    {
        var urlHelperMock = new Mock<IUrlHelper>();
        controller.Url = urlHelperMock.Object;
        return urlHelperMock;
    }

    public static Mock<IUrlHelper> AddUrlForRoute(this Mock<IUrlHelper> urlHelperMock, string routeName, string url = TestConstants.DefaultUrl)
    {
        urlHelperMock
            .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c => c.RouteName!.Equals(routeName))))
            .Returns(url);
        return urlHelperMock;
    }

    public static Controller AddControllerContextWithAllRoles(this Controller controller)
        => AddControllerContextWithRoles(controller, new[] { Roles.ManageEventsRole, Roles.ManageMembersRole });

    public static Controller AddControllerContextWithRoles(this Controller controller, string[] roles)
    {
        ClaimsIdentity identity = new();
        foreach (var role in roles)
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, role));
        }

        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) } };
        return controller;
    }
}