using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SFA.DAS.Aan.SharedUi.Models;
using SFA.DAS.Admin.Aan.Web.Configuration;
using SFA.DAS.Admin.Aan.Web.Controllers;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.UnitTests.TestHelpers;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers.ErrorControllerTests;
[TestFixture]
public class ErrorControllerTests
{
    const string PageNotFoundViewName = "PageNotFound";
    const string ErrorInServiceViewName = "ErrorInService";
    const string AccessDeniedViewName = "AccessDenied";
    const string ResourceEnvironmentName = "test";
    const string Upn = "upn";
    const string Role = "Admin";
    private static string AdministratorHubUrl = Guid.NewGuid().ToString();
    private Mock<IConfiguration> _mockConfiguration = null!;

    [TestCase(403, AccessDeniedViewName)]
    [TestCase(404, PageNotFoundViewName)]
    [TestCase(500, ErrorInServiceViewName)]
    public void HttpStatusCodeHandler_ReturnsRespectiveView(int statusCode, string expectedViewName)
    {
        // Arrange
        Mock<HttpContext> httpContextMock = new();
        var authorisedUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
           new Claim(ClaimTypes.Upn, Upn)
        }, "mock"));
        httpContextMock.Setup(c => c.User).Returns(authorisedUser);
        _mockConfiguration = new Mock<IConfiguration>();
        var appConfig = new ApplicationConfiguration { UseDfESignIn = true };
        var mockIOptions = new Mock<IOptions<ApplicationConfiguration>>();
        mockIOptions.Setup(ap => ap.Value).Returns(appConfig);
        _mockConfiguration.Setup(x => x["ResourceEnvironmentName"]).Returns(ResourceEnvironmentName);

        var sut = new ErrorController(Mock.Of<ILogger<ErrorController>>(), _mockConfiguration.Object, mockIOptions.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object,
            }
        };
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AdministratorHub, AdministratorHubUrl);

        // Act
        ViewResult result = (ViewResult)sut.HttpStatusCodeHandler(statusCode);

        // Assert
        result.ViewName.Should().Contain(expectedViewName);
    }

    [Test]
    public void HttpStatusCodeHandler_AccessDeniedAndResourceEnvironmentNameIsNull_ReturnsRespectiveView()
    {
        // Arrange
        Mock<HttpContext> httpContextMock = new();
        var authorisedUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
           new Claim(ClaimTypes.Upn, Upn),
           new Claim(ClaimTypes.Role, Role)
        }, "mock"));
        httpContextMock.Setup(c => c.User).Returns(authorisedUser);
        var appConfig = new ApplicationConfiguration { UseDfESignIn = true };
        var mockIOptions = new Mock<IOptions<ApplicationConfiguration>>();
        mockIOptions.Setup(ap => ap.Value).Returns(appConfig);

        var sut = new ErrorController(Mock.Of<ILogger<ErrorController>>(), Mock.Of<IConfiguration>(), mockIOptions.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object,
            }
        };
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AdministratorHub, AdministratorHubUrl);

        // Act
        ViewResult result = (ViewResult)sut.HttpStatusCodeHandler(403);

        // Assert
        result.ViewName.Should().Contain(AccessDeniedViewName);
    }

    [Test]
    public void HttpStatusCodeHandler_InternalServerError_ReturnsRespectiveView()
    {
        // Arrange
        var sut = new ErrorController(Mock.Of<ILogger<ErrorController>>(), Mock.Of<IConfiguration>(), Mock.Of<IOptions<ApplicationConfiguration>>());
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AdministratorHub, AdministratorHubUrl);

        // Act
        ViewResult result = (ViewResult)sut.HttpStatusCodeHandler(500);
        var viewModel = result!.Model as ErrorViewModel;

        // Assert
        Assert.That(viewModel, Is.InstanceOf<ErrorViewModel>());
    }

    [Test]
    public void HttpStatusCodeHandler_InternalServerError_ShouldReturnExpectedValue()
    {
        // Arrange
        var sut = new ErrorController(Mock.Of<ILogger<ErrorController>>(), Mock.Of<IConfiguration>(), Mock.Of<IOptions<ApplicationConfiguration>>());
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AdministratorHub, AdministratorHubUrl);

        // Act
        ViewResult result = (ViewResult)sut.HttpStatusCodeHandler(500);
        var viewModel = result!.Model as ErrorViewModel;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(viewModel!.HomePageUrl, Is.EqualTo(AdministratorHubUrl));
        });
    }
}
