using System.Globalization;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Aan.SharedUi.Models.AmbassadorProfile;
using SFA.DAS.Admin.Aan.Application.OuterApi.Members.Responses;
using SFA.DAS.Admin.Aan.Application.OuterApi.Profiles;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Controllers.ManageMembers;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models;
using SFA.DAS.Admin.Aan.Web.UnitTests.TestHelpers;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers;

public class MemberProfileControllerTests
{
    private readonly string RemoveMemberUrl = Guid.NewGuid().ToString();
    private readonly Guid userId = Guid.NewGuid();
    private readonly Guid memberId = Guid.NewGuid();
    private MemberProfileResponse memberProfileResponse = null!;
    private GetProfilesResult getProfilesResult = null!;
    private MemberProfileController sut = null!;
    private IActionResult result = null!;
    private Mock<ISessionService> sessionServiceMock = null!;
    private Mock<IOuterApiClient> outerApiClientMock = null!;
    private CancellationToken cancellationToken;

    [SetUp]
    public async Task Setup()
    {
        cancellationToken = new();
        sessionServiceMock = new Mock<ISessionService>();
        outerApiClientMock = new Mock<IOuterApiClient>();
        sessionServiceMock.Setup(s => s.GetMemberId()).Returns(userId);
        Fixture fixture = new();
        memberProfileResponse = fixture.Create<MemberProfileResponse>();
        getProfilesResult = fixture.Create<GetProfilesResult>();
        memberProfileResponse.Preferences = Enumerable.Range(1, 4).Select(id => new MemberPreference { PreferenceId = id, Value = true });
        outerApiClientMock.Setup(c => c.GetMemberProfile(memberId, userId, cancellationToken)).ReturnsAsync(memberProfileResponse);
        outerApiClientMock.Setup(c => c.GetProfilesByUserType(memberProfileResponse.UserType, cancellationToken)).ReturnsAsync(getProfilesResult);
        sut = new(sessionServiceMock.Object, outerApiClientMock.Object);
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.RemoveMember, RemoveMemberUrl);
        result = await sut.Get(memberId, cancellationToken);
    }

    [Test]
    public void Get_ReturnsMemberProfileViewResult() =>
        result.As<ViewResult>().Model.As<AmbassadorProfileViewModel>().Should().NotBeNull();

    [Test]
    public void Get_ShouldInvokeGetMemberProfile() =>
        outerApiClientMock.Verify(a => a.GetMemberProfile(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);

    [Test]
    public void Get_ShouldInvokeGetProfilesByUserType() =>
        outerApiClientMock.Verify(a => a.GetProfilesByUserType(It.IsAny<MemberUserType>(), It.IsAny<CancellationToken>()), Times.Once);

    [Test]
    public void Get_ReturnsView() =>
        result.Should().BeOfType<ViewResult>();

    [Test]
    public void Get_ShouldReturnExpectedValueForActivities()
    {
        // Act
        var viewResult = result as ViewResult;
        var viewModel = viewResult!.Model as AmbassadorProfileViewModel;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(viewModel!.Activities, Is.Not.Null);
            Assert.That(viewModel!.Activities.FutureEvents, Is.Not.Null);
            Assert.That(viewModel!.Activities.FutureEvents.Count, Is.EqualTo(memberProfileResponse.Activities.EventsPlanned.Events.Count));
            Assert.That(viewModel!.Activities.FutureEventsCount, Is.EqualTo(memberProfileResponse.Activities.EventsPlanned.Events.Count));
            Assert.That(viewModel!.Activities.FutureEvents[0].EventDate, Is.EqualTo(memberProfileResponse.Activities.EventsPlanned.Events[0].EventDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)));
            Assert.That(viewModel!.Activities.FutureEvents[0].EventTitle, Is.EqualTo(memberProfileResponse.Activities.EventsPlanned.Events[0].EventTitle));
        });
    }

    [Test]
    public void Get_ShouldReturnExpectedValueForRemoveMember()
    {
        // Act
        var viewResult = result as ViewResult;
        var viewModel = viewResult!.Model as AmbassadorProfileViewModel;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(viewModel!.RemoveMember, Is.Not.Null);
            Assert.That(viewModel!.RemoveMember.FirstName, Is.EqualTo(memberProfileResponse.FirstName));
            Assert.That(viewModel!.RemoveMember.RouteLink, Is.EqualTo(RemoveMemberUrl));
        });
    }
}
