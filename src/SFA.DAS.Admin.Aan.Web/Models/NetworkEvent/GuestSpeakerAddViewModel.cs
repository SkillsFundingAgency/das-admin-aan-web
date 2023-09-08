using SFA.DAS.Admin.Aan.Web.Infrastructure;

namespace SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

public class GuestSpeakerAddViewModel : ICancelLink, IPostLink
{
    public string CancelLink { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? JobRoleAndOrganisation { get; set; }
    public int? Id { get; set; }

    public string PostLink { get; set; } = string.Empty;
}