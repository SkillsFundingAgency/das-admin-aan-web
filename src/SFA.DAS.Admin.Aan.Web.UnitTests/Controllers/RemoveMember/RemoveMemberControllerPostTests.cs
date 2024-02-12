using AutoFixture;
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

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers.RemoveMember;
public class RemoveMemberControllerPostTests
{
    private RemoveMemberController sut = null!;
    private Mock<IOuterApiClient> _outerApiMock = null!;
    private Mock<ISessionService> _sessionServiceMock = null!;
    private Mock<IValidator<SubmitRemoveMemberModel>> _validatorMock = null!;
    private Guid memberId = Guid.NewGuid();
    private string MemberProfileUrl = Guid.NewGuid().ToString();
    private string NetworkDirectoryUrl = Guid.NewGuid().ToString();
    private MemberProfileResponse memberProfileResponse = null!;
    private SubmitRemoveMemberModel submitRemoveMemberModel = null!;

    [Test]
    public async Task Index_InvalidCommand_ShouldReturnsView()
    {
        // Arrange
        SetUpModelValidateFalse();

        // Act
        var response = await sut.Index(memberId, submitRemoveMemberModel, CancellationToken.None);
        var result = response as ViewResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response, Is.InstanceOf<ViewResult>());
            Assert.That(result, Is.Not.Null);
        });
    }

    [Test]
    public async Task Index_InvalidCommand_ShouldInvokeGetMemberProfile()
    {
        // Arrange
        SetUpModelValidateFalse();

        // Act
        var response = await sut.Index(memberId, submitRemoveMemberModel, CancellationToken.None);

        // Assert
        _outerApiMock.Verify(x => x.GetMemberProfile(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Index_InvalidCommand_ShouldReturnRemoveMemberViewModel()
    {
        // Arrange
        SetUpModelValidateFalse();

        // Act
        var result = await sut.Index(memberId, submitRemoveMemberModel, CancellationToken.None);
        var viewResult = result as ViewResult;
        var viewModel = viewResult!.Model as RemoveMemberViewModel;

        // Assert
        Assert.That(viewModel, Is.InstanceOf<RemoveMemberViewModel>());
    }

    [Test]
    public async Task Index_InvalidCommand_ShouldReturnExpectedValue()
    {
        // Arrange
        SetUpModelValidateFalse();

        // Act
        var result = await sut.Index(memberId, submitRemoveMemberModel, CancellationToken.None);
        var viewResult = result as ViewResult;
        var viewModel = viewResult!.Model as RemoveMemberViewModel;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(viewModel!.FullName, Is.EqualTo(memberProfileResponse.FullName));
            Assert.That(viewModel!.CancelLink, Is.EqualTo(MemberProfileUrl));
            Assert.That(viewModel!.MemberId, Is.EqualTo(memberId));
            Assert.That(viewModel!.HasRemoveConfirmed, Is.EqualTo(submitRemoveMemberModel.HasRemoveConfirmed));
            Assert.That(viewModel!.Status, Is.EqualTo(submitRemoveMemberModel.Status));
            Assert.That(viewModel!.FirstName, Is.Null);
            Assert.That(viewModel!.RouteLink, Is.Null);
        });
    }

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
        Assert.Multiple(() =>
        {
            Assert.That(response, Is.TypeOf<RedirectToActionResult>());
            var redirectToAction = (RedirectToActionResult)response;
            Assert.That(redirectToAction.ActionName, Does.Contain("RemoveMemberConfirmation"));
        });
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
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<SubmitRemoveMemberModel>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
    }

    private void SetUpModelValidateFalse()
    {
        SetUpControllerWithContext();
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<SubmitRemoveMemberModel>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult(new List<ValidationFailure>()
            {
                new ValidationFailure("TestField","Test Message"){ErrorCode = "1001"}
            }));
    }

    [TearDown]
    public void TearDown()
    {
        if (sut != null) sut.Dispose();
    }
}
