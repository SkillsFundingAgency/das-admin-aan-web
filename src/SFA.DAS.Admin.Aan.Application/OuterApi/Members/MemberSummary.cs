using SFA.DAS.Aan.SharedUi.Constants;
using SFA.DAS.Aan.SharedUi.Models.AmbassadorProfile;
using SFA.DAS.Aan.SharedUi.Models.NetworkDirectory;

namespace SFA.DAS.Admin.Aan.Application.OuterApi.Members;

public record MemberSummary(Guid MemberId, string FullName, int? RegionId, string? RegionName, MemberUserType UserType, bool IsRegionalChair, DateTime JoinedDate)
{
    public const string MultiRegional = "Multi-regional";

    public static implicit operator MembersViewModel(MemberSummary source)
        => new()
        {
            MemberId = source.MemberId,
            FullName = source.FullName,
            RegionId = source.RegionId,
            RegionName = (source.RegionId != null) ? source.RegionName : MultiRegional,
            UserRole = source.IsRegionalChair ? Role.RegionalChair : Enum.Parse<Role>(source.UserType.ToString()),
            IsRegionalChair = source.IsRegionalChair,
            JoinedDate = source.JoinedDate
        };
}
