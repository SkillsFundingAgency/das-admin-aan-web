using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Aan.SharedUi.Infrastructure;
using SFA.DAS.Aan.SharedUi.Models.AmbassadorProfile;
using SFA.DAS.Aan.SharedUi.Models.PublicProfile;
using SFA.DAS.Aan.SharedUi.Services;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Authentication;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models;

namespace SFA.DAS.Admin.Aan.Web.Controllers.ManageMembers;

[Authorize(Roles = Roles.ManageMembersRole)]
[Route("member-profile", Name = RouteNames.MemberProfile)]

public class MemberProfileController : Controller
{
    public const string MemberProfileViewPath = "~/Views/MemberProfile/MemberProfile.cshtml";

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
        var model = await GetViewModel(id, adminMemberId, cancellationToken);
        return View(MemberProfileViewPath, model);
    }

    private async Task<AmbassadorProfileViewModel> GetViewModel(Guid id, Guid adminUserId, CancellationToken cancellationToken)
    {
        var memberProfiles = await _outerApiClient.GetMemberProfile(id, adminUserId, cancellationToken);
        var profilesResult = await _outerApiClient.GetProfilesByUserType(memberProfiles.UserType, cancellationToken);

        AmbassadorProfileViewModel ambassadorProfileViewModel = new();

        ambassadorProfileViewModel.ContactInformation.LinkedInUrl = MemberProfileHelper.GetLinkedInUrl(profilesResult.Profiles, memberProfiles.Profiles);
        ambassadorProfileViewModel.ContactInformation.FirstName = memberProfiles.FirstName;
        ambassadorProfileViewModel.ContactInformation.Email = memberProfiles.Email;

        ambassadorProfileViewModel.MemberInformation.FullName = memberProfiles.FullName;
        ambassadorProfileViewModel.MemberInformation.RegionName = (memberProfiles.RegionId != null) ? memberProfiles.RegionName : "Multi-regional";

        ambassadorProfileViewModel.MemberInformation.ShowMaturityTag = true;
        ambassadorProfileViewModel.MemberInformation.ShowJoinedDate = true;
        ambassadorProfileViewModel.MemberInformation.JoinedDate = memberProfiles.JoinedDate;


        ambassadorProfileViewModel.MemberInformation.UserRole = memberProfiles.UserType.ConvertToRole(memberProfiles.IsRegionalChair);
        ambassadorProfileViewModel.MemberInformation.Biography = MemberProfileHelper.GetProfileValueByDescription(MemberProfileConstants.MemberProfileDescription.Biography, profilesResult.Profiles, memberProfiles.Profiles);
        ambassadorProfileViewModel.MemberInformation.JobTitle = MemberProfileHelper.GetProfileValueByDescription(MemberProfileConstants.MemberProfileDescription.JobTitle, profilesResult.Profiles, memberProfiles.Profiles);

        ambassadorProfileViewModel.ApprenticeshipInformation.IsEmployerInformationAvailable = memberProfiles.UserType == MemberUserType.Employer;
        ambassadorProfileViewModel.ApprenticeshipInformation.IsApprenticeshipInformationAvailable = memberProfiles.UserType == MemberUserType.Apprentice;

        if (ambassadorProfileViewModel.ApprenticeshipInformation.IsEmployerInformationAvailable || ambassadorProfileViewModel.ApprenticeshipInformation.IsApprenticeshipInformationAvailable)
        {
            ambassadorProfileViewModel.ApprenticeshipInformation.ApprenticeshipSectionTitle = MemberProfileHelper.GetApprenticeshipSectionTitle(memberProfiles.UserType, memberProfiles.FirstName);
            ambassadorProfileViewModel.ApprenticeshipInformation.EmployerName = memberProfiles.OrganisationName;
            ambassadorProfileViewModel.ApprenticeshipInformation.EmployerAddress = MemberProfileHelper.GetEmployerAddress(memberProfiles.Profiles);

            if (memberProfiles.Apprenticeship != null)
            {
                //following are only applicable to Apprentice user, the values are assumed to be null otherwise
                ambassadorProfileViewModel.ApprenticeshipInformation.Sector = memberProfiles.Apprenticeship.Sector;
                ambassadorProfileViewModel.ApprenticeshipInformation.Programme = memberProfiles.Apprenticeship.Programme;
                ambassadorProfileViewModel.ApprenticeshipInformation.Level = memberProfiles.Apprenticeship.Level;

                //following are only applicable to Employer user, the values are assumed to be null otherwise
                ambassadorProfileViewModel.ApprenticeshipInformation.Sectors = memberProfiles.Apprenticeship.Sectors;
                ambassadorProfileViewModel.ApprenticeshipInformation.ActiveApprenticesCount = memberProfiles.Apprenticeship.ActiveApprenticesCount;
            }
        }

        ambassadorProfileViewModel.AreasOfInterest = MemberProfileHelper.CreateAreasOfInterestViewModel(memberProfiles.UserType, profilesResult.Profiles, memberProfiles.Profiles, memberProfiles.FirstName);

        return ambassadorProfileViewModel;
    }
}
