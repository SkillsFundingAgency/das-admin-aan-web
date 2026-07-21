using AutoFixture;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Aan.SharedUi.Infrastructure;
using SFA.DAS.Admin.Aan.Application.OuterApi.Members;
using SFA.DAS.Admin.Aan.Application.OuterApi.Members.Responses;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Controllers.ManageMembers;
using SFA.DAS.Admin.Aan.Web.Models.RemoveMember;
using SFA.DAS.Admin.Aan.Web.UnitTests.TestHelpers;
using SFA.DAS.Admin.Aan.Web.Validators;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers.RemoveMember;

public class RemoveMemberControllerPostTests
{
    private RemoveMemberController sut = null!;
    private Mock<IOuterApiClient> _outerApiMock = null!;
    private Mock<ISessionService> _sessionServiceMock = null!;
    private Mock<IValidator<SubmitRemoveMemberModel>> _validatorMock = null!;
    private readonly Guid memberId = Guid.NewGuid();
    private readonly string MemberProfileUrl = Guid.NewGuid().ToString();
    private MemberProfileResponse memberProfileResponse = null!;
    private SubmitRemoveMemberModel submitRemoveMemberModel = null!;

    [Test]
    public async Task Index_PostValidCommand_ShouldInvokePostMemberLeaving()
    {
        // Arrange
        SetUpModelValidateTrue();

        // Act
        var result = await sut.Index(memberId, submitRemoveMemberModel, CancellationToken.None);

        // Assert
        _outerApiMock.Verify(x => x.PostMemberLeaving(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<PostMemberStatusModel>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Index_PostValidCommand_RedirectToNetworkDirectory()
    {
        // Arrange
        SetUpModelValidateTrue();

        // Act
        var response = await sut.Index(memberId, submitRemoveMemberModel, CancellationToken.None);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response, Is.TypeOf<RedirectToActionResult>());
            var redirectToAction = (RedirectToActionResult)response;
            Assert.That(redirectToAction.ActionName, Does.Contain("RemoveMemberConfirmation"));
        }
    }

    [Test]
    public async Task Index_PostModelIsInvalid_ReturnsToRemoveMemberPage()
    {
        var validatorMock = new Mock<IValidator<SubmitRemoveMemberModel>>();

        validatorMock.Setup(x => x.Validate(It.IsAny<SubmitRemoveMemberModel>())).Returns(new ValidationResult
        { Errors = new List<ValidationFailure> { new("testProperty", "testMessage") } });

        var outerApiClientMock = new Mock<IOuterApiClient>();

        outerApiClientMock
            .Setup(x => x.GetMemberProfile(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MemberProfileResponse { FullName = "TestName" });

        var sut = new RemoveMemberController(Mock.Of<ISessionService>(), outerApiClientMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(SharedRouteNames.MemberProfile, MemberProfileUrl).AddUrlForRoute(SharedRouteNames.NetworkDirectory, Guid.NewGuid().ToString());

        var result = await sut.Index(Guid.NewGuid(), new SubmitRemoveMemberModel(), CancellationToken.None) as ViewResult;

        result!.ViewName.Should().Be(RemoveMemberController.ViewPath);
    }

    private void SetUpControllerWithContext()
    {
        var _fixture = new Fixture();
        _outerApiMock = new();
        _sessionServiceMock = new();
        _validatorMock = new();
        memberProfileResponse = _fixture.Create<MemberProfileResponse>();
        submitRemoveMemberModel = _fixture.Create<SubmitRemoveMemberModel>();
        _sessionServiceMock.Setup(x => x.GetMemberId()).Returns(memberId);

        _outerApiMock.Setup(o => o.GetMemberProfile(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(memberProfileResponse);
        _outerApiMock.Setup(o => o.PostMemberLeaving(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<PostMemberStatusModel>(), It.IsAny<CancellationToken>())).ReturnsAsync(string.Empty);

        sut = new RemoveMemberController(_sessionServiceMock.Object, _outerApiMock.Object, _validatorMock.Object);
        sut.AddUrlHelperMock().AddUrlForRoute(SharedRouteNames.MemberProfile, MemberProfileUrl);
    }

    private void SetUpModelValidateTrue()
    {
        SetUpControllerWithContext();
        _validatorMock.Setup(v => v.Validate(It.IsAny<SubmitRemoveMemberModel>())).Returns(new ValidationResult());
    }

    [TearDown]
    public void TearDown()
    {
        sut?.Dispose();
    }
}
