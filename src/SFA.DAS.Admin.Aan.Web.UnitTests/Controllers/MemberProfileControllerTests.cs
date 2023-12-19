using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Aan.SharedUi.Models.AmbassadorProfile;
using SFA.DAS.Aan.SharedUi.OuterApi.Responses;
using SFA.DAS.Admin.Aan.Application.OuterApi.Profiles;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Controllers.ManageMembers;
using SFA.DAS.Admin.Aan.Web.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers;

public class MemberProfileControllerTests
{
    [Test, MoqAutoData]
    public async Task Get_ReturnsMemberProfileViewResult(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IOuterApiClient> clientMock,
        [Greedy] MemberProfileController sut,
        Guid userId,
        Guid memberId,
        GetMemberProfileResponse getMemberProfileResponse,
        GetProfilesResult getProfilesResult,
        CancellationToken cancellationToken)
    {
        sessionServiceMock.Setup(s => s.GetMemberId()).Returns(userId);

        getMemberProfileResponse.Preferences = Enumerable.Range(1, 4).Select(id => new MemberPreference { PreferenceId = id, Value = true });
        clientMock.Setup(c => c.GetMemberProfile(memberId, userId, cancellationToken)).ReturnsAsync(getMemberProfileResponse);
        clientMock.Setup(c => c.GetProfilesByUserType(getMemberProfileResponse.UserType, cancellationToken)).ReturnsAsync(getProfilesResult);

        var result = await sut.Get(memberId, cancellationToken);

        result.As<ViewResult>().Model.As<AmbassadorProfileViewModel>().Should().NotBeNull();
    }
}
