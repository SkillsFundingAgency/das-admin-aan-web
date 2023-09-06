using SFA.DAS.Admin.Aan.Web.Infrastructure;

namespace SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

public class CreateEventGuestSpeakerListViewModel : IBackLink
{
    public string BackLink { get; set; } = null!;
    public List<GuestSpeaker> GuestSpeakers { get; set; } = new List<GuestSpeaker>();

}


public record GuestSpeaker(string Name, int JobRoleAndOrganisation, int Id);