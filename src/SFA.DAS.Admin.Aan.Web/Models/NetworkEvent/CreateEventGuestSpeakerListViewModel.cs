using SFA.DAS.Admin.Aan.Web.Infrastructure;

namespace SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

public class CreateEventGuestSpeakerListViewModel : ICancelLink, IPostLink
{
    public string CancelLink { get; set; } = null!;
    public List<GuestSpeaker> GuestSpeakers { get; set; } = new List<GuestSpeaker>();

    public string PostLink { get; set; } = null!;
    public string AddGuestSpeakerLink { get; set; } = null!;
    public string DeleteSpeakerLink { get; set; } = null!;
}