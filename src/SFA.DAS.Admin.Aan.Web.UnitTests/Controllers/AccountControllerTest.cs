using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SFA.DAS.Admin.Aan.Web.Configuration;
using SFA.DAS.Admin.Aan.Web.Controllers;
using SFA.DAS.Admin.Aan.Web.Models.Account;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers
{
    [TestFixture]
    public class AccountControllerTest
    {
        private AccountController _controller = null!;
        private Mock<ILogger<AccountController>> _mockLogger = null!;
        private Mock<IConfiguration> _mockConfiguration = null!;

        [SetUp]
        public void Setup()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<AccountController>>();
        }


        [TestCase("test", "https://test-services.signin.education.gov.uk/approvals/select-organisation?action=request-service", true)]
        [TestCase("pp", "https://test-services.signin.education.gov.uk/approvals/select-organisation?action=request-service", true)]
        [TestCase("local", "https://test-services.signin.education.gov.uk/approvals/select-organisation?action=request-service", false)]
        [TestCase("prd", "https://services.signin.education.gov.uk/approvals/select-organisation?action=request-service", false)]
        public void When_AccessDenied_Then_ViewIsReturned(string env, string helpLink, bool useDfESignIn)
        {
            //arrange
            var appConfig = new ApplicationConfiguration { UseDfESignIn = useDfESignIn };
            var mockIOptions = new Mock<IOptions<ApplicationConfiguration>>();
            mockIOptions.Setup(ap => ap.Value).Returns(appConfig);
            _mockConfiguration.Setup(x => x["ResourceEnvironmentName"]).Returns(env);
            _controller = new AccountController(_mockLogger.Object, mockIOptions.Object, _mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext()
            };
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            //sut
            var result = (ViewResult)_controller.AccessDenied();

            //assert
            result.Should().NotBeNull();
            result.ViewName.Should().Be(nameof(AccountController.AccessDenied));

            var actualModel = result.Model as Error403ViewModel;
            actualModel.Should().NotBeNull();
            actualModel?.HelpPageLink.Should().Be(helpLink);
            actualModel?.UseDfESignIn.Should().Be(useDfESignIn);
        }
    }
}
