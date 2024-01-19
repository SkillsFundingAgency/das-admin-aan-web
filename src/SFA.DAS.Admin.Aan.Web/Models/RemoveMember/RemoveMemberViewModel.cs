namespace SFA.DAS.Admin.Aan.Web.Models.RemoveMember;

public class RemoveMemberViewModel : SubmitRemoveMemberModel
{
    public string? FirstName { get; set; }
    public string? RouteLink { get; set; }
    public string FullName { get; set; } = null!;
    public string? CancelLink { get; set; }
    public Guid MemberId { get; set; }
}
