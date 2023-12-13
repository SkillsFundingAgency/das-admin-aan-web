using SFA.DAS.Aan.SharedUi.Models.AmbassadorProfile;

namespace SFA.DAS.Admin.Aan.Application.OuterApi.Members;

public class GetMemberProfileResponse
{
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? OrganisationName { get; set; }
    public int? RegionId { get; set; }
    public string RegionName { get; set; } = null!;
    public MemberUserType UserType { get; set; }
    public bool IsRegionalChair { get; set; }
    public ApprenticeshipDetails Apprenticeship { get; set; } = new();
    public IEnumerable<MemberProfile> Profiles { get; set; } = null!;
    public IEnumerable<MemberPreference> Preferences { get; set; } = null!;
}

public class ApprenticeshipDetails
{
    public string? Sector { get; set; } = null!;
    public string? Programme { get; set; } = null!;
    public string? Level { get; set; } = null!;
    public List<string> Sectors { get; set; } = new List<string>();
    public int? ActiveApprenticesCount { get; set; }
}
