using SFA.DAS.Admin.Aan.Web.Infrastructure;

namespace SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

public class CreateEventGuestSpeakerListViewModel : IEventPageEditFields
{
    public List<GuestSpeaker> GuestSpeakers { get; set; } = new List<GuestSpeaker>();
    public string AddGuestSpeakerLink { get; set; } = null!;
    public string DeleteSpeakerLink { get; set; } = null!;
    public string? PageTitle { get; set; }
    public string? PostLink { get; set; }
    public string? CancelLink { get; set; }
}