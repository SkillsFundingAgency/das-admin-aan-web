using SFA.DAS.Admin.Aan.Application.Constants;

namespace SFA.DAS.Admin.Aan.Web.Models.RemoveMember;

public class SubmitRemoveMemberModel
{
    public MembershipStatusType Status { get; set; }
    public bool HasRemoveConfirmed { get; set; }
}
