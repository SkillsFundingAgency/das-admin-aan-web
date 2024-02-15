using System.Globalization;
using AutoFixture;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Aan.SharedUi.Models.AmbassadorProfile;
using SFA.DAS.Admin.Aan.Application.OuterApi.Members;
using SFA.DAS.Admin.Aan.Application.OuterApi.Members.Responses;
using SFA.DAS.Admin.Aan.Application.OuterApi.Profiles;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Controllers.ManageMembers;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models;
using SFA.DAS.Admin.Aan.Web.UnitTests.TestHelpers;
using static SFA.DAS.Aan.SharedUi.Constants.ProfileConstants;

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
    public void Setup()
    {
        cancellationToken = new();
        sessionServiceMock = new Mock<ISessionService>();
        outerApiClientMock = new Mock<IOuterApiClient>();
        sessionServiceMock.Setup(s => s.GetMemberId()).Returns(userId);

    }

    [Test]
    public async Task Get_ReturnsMemberProfileViewResult()
    {
        // Arrange
        await Init();

        // Assert
        result.As<ViewResult>().Model.As<AmbassadorProfileViewModel>().Should().NotBeNull();
    }


    [Test]
    public async Task Get_ShouldInvokeGetMemberProfile()
    {
        // Arrange
        await Init();

        // Assert
        outerApiClientMock.Verify(a => a.GetMemberProfile(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Get_ShouldInvokeGetProfilesByUserType()
    {
        // Arrange
        await Init();

        // Assert
        outerApiClientMock.Verify(a => a.GetProfilesByUserType(It.IsAny<MemberUserType>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Get_ReturnsView()
    {
        // Arrange
        await Init();

        // Assert
        result.Should().BeOfType<ViewResult>();
    }

    [Test]
    public async Task Get_ShouldReturnExpectedValueForActivities()
    {
        // Arrange 
        await Init();
        int eventsAttendedCount = memberProfileResponse.Activities.EventsAttended.Events.Count(x => x.Urn == null);
        int schoolEventsAttendedCount = memberProfileResponse.Activities.EventsAttended.Events.Count(x => x.Urn != null);
        string lastSignedUpDate = (memberProfileResponse.Activities.LastSignedUpDate != null ? memberProfileResponse.Activities.LastSignedUpDate?.ToString("dd/MM/yyyy") : "no events attended")!;

        // Act
        var viewResult = result as ViewResult;
        var viewModel = viewResult!.Model as AmbassadorProfileViewModel;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(viewModel!.Activities, Is.Not.Null);
            Assert.That(viewModel!.Activities.FutureEvents, Is.Not.Null);
            Assert.That(viewModel!.Activities.FutureEvents, Has.Count.EqualTo(memberProfileResponse.Activities.EventsPlanned.Events.Count));
            Assert.That(viewModel!.Activities.FutureEventsCount, Is.EqualTo(memberProfileResponse.Activities.EventsPlanned.Events.Count));
            Assert.That(viewModel!.Activities.FirstName, Is.EqualTo(memberProfileResponse.FirstName));
            Assert.That(viewModel!.Activities.EventsAttendedCount, Is.EqualTo(eventsAttendedCount));
            Assert.That(viewModel!.Activities.SchoolEventsAttendedCount, Is.EqualTo(schoolEventsAttendedCount));
            Assert.That(viewModel!.Activities.LastEventSignUpDate, Is.EqualTo(lastSignedUpDate));
        });
    }

    [Test]
    public async Task Get_ShouldReturnExpectedValueForRemoveMember()
    {
        // Arrange 
        await Init();

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

    [Test]
    public async Task Get_ShouldReturnUpcomingEventsInEventDateOrder()
    {
        // Arrange
        await Init();
        List<EventViewModel> eventViewModels = memberProfileResponse.Activities.EventsPlanned.Events.OrderBy(x => x.EventDate).Select((x) => new EventViewModel(x.EventTitle, x.EventDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture))).ToList();

        // Act
        var viewResult = result as ViewResult;
        var viewModel = viewResult!.Model as AmbassadorProfileViewModel;

        // Assert
        Assert.That(viewModel!.Activities.FutureEvents, Is.EqualTo(eventViewModels));
    }

    [Test]
    public async Task Get_MemberWithNullRegion_ShouldReturnExpectedValueForRegionName()
    {
        // Arrange 
        Fixture fixture = new();
        memberProfileResponse = fixture.Create<MemberProfileResponse>();
        getProfilesResult = fixture.Create<GetProfilesResult>();
        memberProfileResponse.RegionId = null;
        memberProfileResponse.Preferences = Enumerable.Range(1, 4).Select(id => new MemberPreference { PreferenceId = id, Value = true });
        outerApiClientMock.Setup(c => c.GetMemberProfile(memberId, userId, cancellationToken)).ReturnsAsync(memberProfileResponse);
        outerApiClientMock.Setup(c => c.GetProfilesByUserType(memberProfileResponse.UserType, cancellationToken)).ReturnsAsync(getProfilesResult);
        sut = new(sessionServiceMock.Object, outerApiClientMock.Object);
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.RemoveMember, RemoveMemberUrl);
        result = await sut.Get(memberId, cancellationToken);

        // Act
        var viewResult = result as ViewResult;
        var viewModel = viewResult!.Model as AmbassadorProfileViewModel;

        // Assert
        Assert.That(viewModel!.MemberInformation.RegionName, Is.EqualTo("Multi-regional"));
    }

    [Test]
    public async Task Get_MemberWithValidRegion_ShouldReturnExpectedValueForRegionName()
    {
        // Arrange 
        await Init();

        // Act
        var viewResult = result as ViewResult;
        var viewModel = viewResult!.Model as AmbassadorProfileViewModel;

        // Assert
        Assert.That(viewModel!.MemberInformation.RegionName, Is.EqualTo(memberProfileResponse.RegionName));
    }

    [Test]
    public async Task Get_MemberWithNoActivity_ShouldReturnExpectedValueForActivities()
    {
        // Arrange 
        Fixture fixture = new();
        memberProfileResponse = fixture.Create<MemberProfileResponse>();
        getProfilesResult = fixture.Create<GetProfilesResult>();
        memberProfileResponse.Activities.EventsAttended.Events = new List<EventAttendanceModel>();
        memberProfileResponse.Activities.EventsPlanned.Events = new List<EventAttendanceModel>();
        memberProfileResponse.Activities.LastSignedUpDate = null;
        memberProfileResponse.Preferences = Enumerable.Range(1, 4).Select(id => new MemberPreference { PreferenceId = id, Value = true });
        outerApiClientMock.Setup(c => c.GetMemberProfile(memberId, userId, cancellationToken)).ReturnsAsync(memberProfileResponse);
        outerApiClientMock.Setup(c => c.GetProfilesByUserType(memberProfileResponse.UserType, cancellationToken)).ReturnsAsync(getProfilesResult);
        sut = new(sessionServiceMock.Object, outerApiClientMock.Object);
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.RemoveMember, RemoveMemberUrl);
        result = await sut.Get(memberId, cancellationToken);

        // Act
        var viewResult = result as ViewResult;
        var viewModel = viewResult!.Model as AmbassadorProfileViewModel;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(viewModel!.Activities.FutureEvents, Has.Count.EqualTo(0));
            Assert.That(viewModel!.Activities.FutureEventsCount, Is.EqualTo(0));
            Assert.That(viewModel!.Activities.EventsAttendedCount, Is.EqualTo(0));
            Assert.That(viewModel!.Activities.SchoolEventsAttendedCount, Is.EqualTo(0));
            Assert.That(viewModel!.Activities.LastEventSignUpDate, Is.EqualTo("no events attended"));
        });
    }

    [Test, AutoData]
    public async Task Get_NewApprenticeAccount_ShouldReturnExpectedValueForReasonForJoining(MemberProfileResponse memberProfileResponse, GetProfilesResult profilesResult, string reasonToJoin, Mock<ISessionService> _sessionServiceMock, Mock<IOuterApiClient> _outerApiClientMock, CancellationToken _cancellationToken)
    {
        // Arrange
        _sessionServiceMock.Setup(s => s.GetMemberId()).Returns(userId);
        memberProfileResponse.UserType = MemberUserType.Apprentice;
        memberProfileResponse.IsRegionalChair = false;
        memberProfileResponse.JoinedDate = DateTime.UtcNow.AddDays(-1);
        memberProfileResponse.Profiles = new List<MemberProfile>() {
        new MemberProfile(){ProfileId=ProfileIds.ReasonToJoinAmbassadorNetwork,Value=reasonToJoin}
        };
        memberProfileResponse.Preferences = Enumerable.Range(1, 4).Select(id => new MemberPreference { PreferenceId = id, Value = true });
        _outerApiClientMock.Setup(c => c.GetMemberProfile(memberId, userId, _cancellationToken)).ReturnsAsync(memberProfileResponse);
        _outerApiClientMock.Setup(c => c.GetProfilesByUserType(memberProfileResponse.UserType, _cancellationToken)).ReturnsAsync(profilesResult);
        sut = new(_sessionServiceMock.Object, _outerApiClientMock.Object);
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.RemoveMember, RemoveMemberUrl);

        // Act
        result = await sut.Get(memberId, _cancellationToken);
        var viewResult = result as ViewResult;
        var viewModel = viewResult!.Model as AmbassadorProfileViewModel;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(viewModel!.ReasonForJoining.ShowReasonForJoining, Is.True);
            Assert.That(viewModel!.ReasonForJoining.ReasonForJoining, Is.EqualTo(reasonToJoin));
        });
    }

    [Test, AutoData]
    public async Task Get_NewApprenticeAccountWithNoValue_ShouldReturnExpectedValueForReasonForJoining(MemberProfileResponse memberProfileResponse, GetProfilesResult profilesResult, Mock<ISessionService> _sessionServiceMock, Mock<IOuterApiClient> _outerApiClientMock, CancellationToken _cancellationToken)
    {
        // Arrange
        _sessionServiceMock.Setup(s => s.GetMemberId()).Returns(userId);
        memberProfileResponse.UserType = MemberUserType.Apprentice;
        memberProfileResponse.IsRegionalChair = false;
        memberProfileResponse.JoinedDate = DateTime.UtcNow.AddDays(-1);
        memberProfileResponse.Profiles = new[] { new MemberProfile() { ProfileId = ProfileIds.ReasonToJoinAmbassadorNetwork, Value = string.Empty } };
        memberProfileResponse.Preferences = Enumerable.Range(1, 4).Select(id => new MemberPreference { PreferenceId = id, Value = true });
        _outerApiClientMock.Setup(c => c.GetMemberProfile(memberId, userId, _cancellationToken)).ReturnsAsync(memberProfileResponse);
        _outerApiClientMock.Setup(c => c.GetProfilesByUserType(memberProfileResponse.UserType, _cancellationToken)).ReturnsAsync(profilesResult);
        sut = new(_sessionServiceMock.Object, _outerApiClientMock.Object);
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.RemoveMember, RemoveMemberUrl);

        // Act
        result = await sut.Get(memberId, _cancellationToken);
        var viewResult = result as ViewResult;
        var viewModel = viewResult!.Model as AmbassadorProfileViewModel;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(viewModel!.ReasonForJoining.ShowReasonForJoining, Is.False);
            Assert.That(viewModel!.ReasonForJoining.ReasonForJoining, Is.EqualTo(string.Empty));
        });
    }

    [Test, AutoData]
    public async Task Get_OldApprenticeAccount_ShouldReturnExpectedValueForReasonForJoining(MemberProfileResponse memberProfileResponse, GetProfilesResult profilesResult, string reasonToJoin, Mock<ISessionService> _sessionServiceMock, Mock<IOuterApiClient> _outerApiClientMock, CancellationToken _cancellationToken)
    {
        // Arrange
        _sessionServiceMock.Setup(s => s.GetMemberId()).Returns(userId);
        memberProfileResponse.UserType = MemberUserType.Apprentice;
        memberProfileResponse.IsRegionalChair = false;
        memberProfileResponse.JoinedDate = DateTime.UtcNow.AddDays(-100);
        memberProfileResponse.Profiles = new[] { new MemberProfile() { ProfileId = ProfileIds.ReasonToJoinAmbassadorNetwork, Value = reasonToJoin } };
        memberProfileResponse.Preferences = Enumerable.Range(1, 4).Select(id => new MemberPreference { PreferenceId = id, Value = true });
        _outerApiClientMock.Setup(c => c.GetMemberProfile(memberId, userId, _cancellationToken)).ReturnsAsync(memberProfileResponse);
        _outerApiClientMock.Setup(c => c.GetProfilesByUserType(memberProfileResponse.UserType, _cancellationToken)).ReturnsAsync(profilesResult);
        sut = new(_sessionServiceMock.Object, _outerApiClientMock.Object);
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.RemoveMember, RemoveMemberUrl);

        // Act
        result = await sut.Get(memberId, _cancellationToken);
        var viewResult = result as ViewResult;
        var viewModel = viewResult!.Model as AmbassadorProfileViewModel;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(viewModel!.ReasonForJoining.ShowReasonForJoining, Is.False);
            Assert.That(viewModel!.ReasonForJoining.ReasonForJoining, Is.EqualTo(reasonToJoin));
        });
    }

    [Test, AutoData]
    public async Task Get_NewEmployerAccount_ShouldReturnNullValueForReasonForJoining(MemberProfileResponse memberProfileResponse, GetProfilesResult profilesResult, Mock<ISessionService> _sessionServiceMock, Mock<IOuterApiClient> _outerApiClientMock, CancellationToken _cancellationToken)
    {
        // Arrange
        _sessionServiceMock.Setup(s => s.GetMemberId()).Returns(userId);
        memberProfileResponse.UserType = MemberUserType.Employer;
        memberProfileResponse.IsRegionalChair = false;
        memberProfileResponse.JoinedDate = DateTime.UtcNow.AddDays(-1);

        memberProfileResponse.Preferences = Enumerable.Range(1, 4).Select(id => new MemberPreference { PreferenceId = id, Value = true });
        _outerApiClientMock.Setup(c => c.GetMemberProfile(memberId, userId, _cancellationToken)).ReturnsAsync(memberProfileResponse);
        _outerApiClientMock.Setup(c => c.GetProfilesByUserType(memberProfileResponse.UserType, _cancellationToken)).ReturnsAsync(profilesResult);
        sut = new(_sessionServiceMock.Object, _outerApiClientMock.Object);
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.RemoveMember, RemoveMemberUrl);

        // Act
        result = await sut.Get(memberId, _cancellationToken);
        var viewResult = result as ViewResult;
        var viewModel = viewResult!.Model as AmbassadorProfileViewModel;

        // Assert
        Assert.That(viewModel!.ReasonForJoining.ReasonForJoining, Is.Null);
    }

    private async Task Init()
    {
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
}