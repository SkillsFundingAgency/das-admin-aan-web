using AutoFixture;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Aan.SharedUi.Infrastructure;
using SFA.DAS.Aan.SharedUi.OuterApi.Responses;
using SFA.DAS.Admin.Aan.Application.Constants;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Controllers.ManageMembers;
using SFA.DAS.Admin.Aan.Web.Models.RemoveMember;
using SFA.DAS.Admin.Aan.Web.UnitTests.TestHelpers;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers.RemoveMember;
public class RemoveMemberControllerGetTests
{
    private RemoveMemberController sut = null!;
    private Mock<IOuterApiClient> _outerApiMock = null!;
    private Mock<ISessionService> _sessionServiceMock = null!;
    private Guid memberId = Guid.NewGuid();
    private string MemberProfileUrl = Guid.NewGuid().ToString();
    private string NetworkDirectoryUrl = Guid.NewGuid().ToString();
    private GetMemberProfileResponse getMemberProfileResponse = null!;
    private IFixture _fixture;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _outerApiMock = new();
        _sessionServiceMock = new();
        getMemberProfileResponse = _fixture.Create<GetMemberProfileResponse>();
        _sessionServiceMock.Setup(x => x.GetMemberId()).Returns(memberId);

        _outerApiMock.Setup(o => o.GetMemberProfile(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(getMemberProfileResponse);

        sut = new RemoveMemberController(_sessionServiceMock.Object, _outerApiMock.Object, Mock.Of<IValidator<SubmitRemoveMemberModel>>());
        sut.AddUrlHelperMock().AddUrlForRoute(SharedRouteNames.MemberProfile, MemberProfileUrl).AddUrlForRoute(SharedRouteNames.NetworkDirectory, NetworkDirectoryUrl);
    }

    [Test]
    public async Task Index_ShouldReturnView()
    {
        // Act
        var result = await sut.Index(memberId, CancellationToken.None);
        var viewResult = result as ViewResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ViewResult>());
            Assert.That(viewResult, Is.Not.Null);
        });
    }

    [Test]
    public async Task Index_ShouldInvokeGetMemberProfile()
    {
        // Act
        var result = await sut.Index(memberId, CancellationToken.None);

        // Assert
        _outerApiMock.Verify(x => x.GetMemberProfile(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Index_ShouldReturnRemoveMemberViewModel()
    {
        // Act
        var result = await sut.Index(memberId, CancellationToken.None);
        var viewResult = result as ViewResult;
        var viewModel = viewResult!.Model as RemoveMemberViewModel;

        // Assert
        Assert.That(viewModel, Is.InstanceOf<RemoveMemberViewModel>());
    }

    [Test]
    public async Task Index_ShouldReturnExpectedValue()
    {
        // Act
        var result = await sut.Index(memberId, CancellationToken.None);
        var viewResult = result as ViewResult;
        var viewModel = viewResult!.Model as RemoveMemberViewModel;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(viewModel!.FullName, Is.EqualTo(getMemberProfileResponse.FullName));
            Assert.That(viewModel!.CancelLink, Is.EqualTo(MemberProfileUrl));
            Assert.That(viewModel!.MemberId, Is.EqualTo(memberId));
            Assert.That(viewModel!.HasRemoveConfirmed, Is.EqualTo(false));
            Assert.That(viewModel!.Status, Is.EqualTo(MembershipStatusType.Live));
            Assert.That(viewModel!.FirstName, Is.Null);
            Assert.That(viewModel!.RouteLink, Is.Null);
        });
    }

    [Test]
    public void RemoveMemberConfirmation_ShouldReturnView()
    {
        // Act
        var result = sut.RemoveMemberConfirmation();
        var viewResult = result as ViewResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ViewResult>());
            Assert.That(viewResult, Is.Not.Null);
        });
    }

    [Test]
    public void RemoveMemberConfirmation_ShouldReturnRemoveMemberConfirmationModel()
    {
        // Act
        var result = sut.RemoveMemberConfirmation();
        var viewResult = result as ViewResult;
        var viewModel = viewResult!.Model as RemoveMemberConfirmationModel;

        // Assert
        Assert.That(viewModel, Is.InstanceOf<RemoveMemberConfirmationModel>());
    }

    [Test]
    public void RemoveMemberConfirmation_ShouldReturnExpectedValue()
    {
        // Act
        var result = sut.RemoveMemberConfirmation();
        var viewResult = result as ViewResult;
        var viewModel = viewResult!.Model as RemoveMemberConfirmationModel;

        // Assert
        Assert.That(viewModel!.NetworkDirectoryLink, Is.EqualTo(NetworkDirectoryUrl));
    }

    [TearDown]
    public void TearDown()
    {
        if (sut != null) sut.Dispose();
    }
}
