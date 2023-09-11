using SFA.DAS.Admin.Aan.Web.Infrastructure;

namespace SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

public class GuestSpeakerAddViewModel : IEventPageEditFields
{
    public string? Name { get; set; }
    public string? JobRoleAndOrganisation { get; set; }
    public int? Id { get; set; }
    public string? PageTitle { get; set; }
    public string? PostLink { get; set; }
    public string? CancelLink { get; set; }
}