using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Aan.SharedUi.Infrastructure;
using SFA.DAS.Aan.SharedUi.Models;
using SFA.DAS.Aan.SharedUi.Models.AmbassadorProfile;
using SFA.DAS.Admin.Aan.Application.OuterApi.Members;
using SFA.DAS.Admin.Aan.Application.OuterApi.Profiles;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Authentication;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using static SFA.DAS.Aan.SharedUi.Constants.ProfileConstants;

namespace SFA.DAS.Admin.Aan.Web.Controllers.ManageMembers;

[Authorize(Roles = Roles.ManageMembersRole)]
[Route("member-profile", Name = RouteNames.MemberProfile)]

public class MemberProfileController : Controller
{
    public const string MemberProfileViewPath = "~/Views/MemberProfile/Profile.cshtml";

    private readonly ISessionService _sessionService;
    private readonly IOuterApiClient _outerApiClient;

    public MemberProfileController(ISessionService sessionService, IOuterApiClient outerApiClient)
    {
        _sessionService = sessionService;
        _outerApiClient = outerApiClient;
    }

    [HttpGet]
    [Route("{id}", Name = SharedRouteNames.MemberProfile)]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var adminMemberId = _sessionService.GetMemberId();
        var memberProfiles = await _outerApiClient.GetMemberProfile(id, adminMemberId, cancellationToken);
        MemberProfileViewModel model = await MemberProfileMapping(memberProfiles, id == adminMemberId, cancellationToken);
        return View(MemberProfileViewPath, model);
    }

    private static List<int> eventsProfileIds = new List<int>()
    {
        ProfileIds.NetworkingAtEventsInPerson,
        ProfileIds.PresentingAtEventsInPerson,
        ProfileIds.PresentingAtHybridEventsOnlineAndInPerson,
        ProfileIds.PresentingAtOnlineEvents,
        ProfileIds.ProjectManagementAndDeliveryOfRegionalEventsOrPlayingARoleInOrganisingNationalEvents
    };
    private static readonly List<int> promotionsProfileIds = new()
    {
        ProfileIds.CarryingOutAndWritingUpCaseStudies,
        ProfileIds.DesigningAndCreatingMarketingMaterialsToChampionTheNetwork,
        ProfileIds.DistributingCommunicationsToTheNetwork,
        ProfileIds.EngagingWithStakeholdersToWorkOutHowToImproveTheNetwork,
        ProfileIds.PromotingTheNetworkOnSocialMediaChannels
    };
    private static readonly List<int> addressProfileIds = new()
    {
        ProfileIds.EmployerAddress1,
        ProfileIds.EmployerAddress2,
        ProfileIds.EmployerTownOrCity,
        ProfileIds.EmployerCounty,
        ProfileIds.EmployerPostcode
    };
    private static readonly List<int> reasonToJoinProfileIds = new()
    {
        ProfileIds.MeetOtherAmbassadorsAndGrowYourNetwork,
        ProfileIds.ShareYourKnowledgeExperienceAndBestPractice,
        ProfileIds.ProjectManageAndDeliverNetworkEvents,
        ProfileIds.BeARoleModelAndActAsAnInformalMentor,
        ProfileIds.ChampionApprenticeshipDeliveryWithinYourNetworks
    };
    private static readonly List<int> supportProfileIds = new()
    {
        ProfileIds.BuildingApprenticeshipProfileOfMyOrganisation,
        ProfileIds.IncreasingEngagementWithSchoolsAndColleges,
        ProfileIds.GettingStartedWithApprenticeships,
        ProfileIds.UnderstandingTrainingProvidersAndResourcesOthersAreUsing,
        ProfileIds.UsingTheNetworkToBestBenefitMyOrganisation
    };
    private static readonly List<int> employerAddressProfileIds = new()
    {
        ProfileIds.EmployerUserEmployerAddress1,
        ProfileIds.EmployerUserEmployerAddress2,
        ProfileIds.EmployerUserEmployerTownOrCity,
        ProfileIds.EmployerUserEmployerCounty,
        ProfileIds.EmployerUserEmployerPostcode
    };

    private async Task<MemberProfileViewModel> MemberProfileMapping(GetMemberProfileResponse memberProfiles, bool isLoggedInUserMemberProfile, CancellationToken cancellationToken)
    {
        MemberProfileDetail memberProfileDetail = MemberProfileDetailMapping(memberProfiles);
        MemberProfileMappingModel memberProfileMappingModel;
        GetProfilesResult profilesResult;
        if (memberProfileDetail.UserType == MemberUserType.Apprentice)
        {
            memberProfileMappingModel = new()
            {
                LinkedinProfileId = ProfileIds.LinkedIn,
                JobTitleProfileId = ProfileIds.JobTitle,
                BiographyProfileId = ProfileIds.Biography,
                FirstSectionProfileIds = eventsProfileIds,
                SecondSectionProfileIds = promotionsProfileIds,
                AddressProfileIds = addressProfileIds,
                EmployerNameProfileId = ProfileIds.EmployerName,
                IsLoggedInUserMemberProfile = isLoggedInUserMemberProfile
            };
            profilesResult = await _outerApiClient.GetProfilesByUserType(MemberUserType.Apprentice.ToString(), cancellationToken);
        }
        else
        {
            memberProfileMappingModel = new()
            {
                LinkedinProfileId = ProfileIds.EmployerLinkedIn,
                JobTitleProfileId = ProfileIds.EmployerJobTitle,
                BiographyProfileId = ProfileIds.EmployerBiography,
                FirstSectionProfileIds = reasonToJoinProfileIds,
                SecondSectionProfileIds = supportProfileIds,
                AddressProfileIds = employerAddressProfileIds,
                EmployerNameProfileId = ProfileIds.EmployerUserEmployerName,
                IsLoggedInUserMemberProfile = isLoggedInUserMemberProfile
            };
            profilesResult = await _outerApiClient.GetProfilesByUserType(MemberUserType.Employer.ToString(), cancellationToken);
        }
        return new(memberProfileDetail, profilesResult.Profiles, memberProfileMappingModel);
    }

    public static MemberProfileDetail MemberProfileDetailMapping(GetMemberProfileResponse memberProfiles)
    {
        MemberProfileDetail memberProfileDetail = new()
        {
            FullName = memberProfiles.FullName,
            Email = memberProfiles.Email,
            FirstName = memberProfiles.FirstName,
            LastName = memberProfiles.LastName,
            OrganisationName = memberProfiles.OrganisationName,
            RegionId = memberProfiles.RegionId ?? 0,
            RegionName = memberProfiles.RegionName,
            UserType = memberProfiles.UserType,
            IsRegionalChair = memberProfiles.IsRegionalChair
        };
        if (memberProfiles.Apprenticeship != null)
        {
            if (memberProfileDetail.UserType == MemberUserType.Employer)
            {
                memberProfileDetail.Sectors = memberProfiles.Apprenticeship!.Sectors;
                memberProfileDetail.ActiveApprenticesCount = memberProfiles.Apprenticeship.ActiveApprenticesCount.GetValueOrDefault();
            }
            if (memberProfileDetail.UserType == MemberUserType.Apprentice)
            {
                memberProfileDetail.Sector = memberProfiles.Apprenticeship.Sector ?? string.Empty;
                memberProfileDetail.Programmes = memberProfiles.Apprenticeship.Programme ?? string.Empty;
                memberProfileDetail.Level = memberProfiles.Apprenticeship.Level ?? string.Empty;
            }
        }
        memberProfileDetail.Profiles = memberProfiles.Profiles;
        return memberProfileDetail;
    }
}
