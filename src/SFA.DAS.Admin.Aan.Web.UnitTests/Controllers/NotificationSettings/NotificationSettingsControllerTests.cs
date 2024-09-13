using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Admin.Aan.Application.OuterApi.NotificationSettings;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Controllers;
using SFA.DAS.Admin.Aan.Web.Models.NotificationSettings;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers.NotificationSettings
{
    [TestFixture]
    public class NotificationSettingsControllerTests
    {
        private Fixture _fixture;
        private Mock<IOuterApiClient> _mockOuterApiClient;
        private Mock<ISessionService> _mockSessionService;
        NotificationSettingsController _controller;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _mockOuterApiClient = new Mock<IOuterApiClient>();
            _mockSessionService = new Mock<ISessionService>();

            _controller = new NotificationSettingsController(_mockOuterApiClient.Object, _mockSessionService.Object);
        }

        [Test]
        public async Task Index_ReturnsViewResult_WithNotificationSettingsViewModel()
        {
            var memberId = _fixture.Create<Guid>();
            var apiResponse = _fixture.Create<GetNotificationSettingsResponse>();

            
            _mockSessionService.Setup(s => s.GetMemberId()).Returns(memberId);
            _mockOuterApiClient.Setup(c => c.GetNotificationSettings(memberId, default))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Which;
            var viewModel = viewResult.Model.Should().BeOfType<NotificationSettingsViewModel>().Which;
            viewModel.Should().BeEquivalentTo((NotificationSettingsViewModel)apiResponse);
        }
    }
}

