using SFA.DAS.Aan.SharedUi.OuterApi.Responses;

namespace SFA.DAS.Admin.Aan.Application.OuterApi.Members.Responses;
public class MemberProfileResponse : GetMemberProfileResponse
{
    public MemberActivities Activities { get; set; } = null!;
}
