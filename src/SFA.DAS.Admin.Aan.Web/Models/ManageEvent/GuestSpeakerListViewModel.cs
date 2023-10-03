namespace SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

public class GuestSpeakerListViewModel : EventPageEditFields
{
    public List<GuestSpeaker> GuestSpeakers { get; set; } = new List<GuestSpeaker>();
    public string AddGuestSpeakerLink { get; set; } = null!;
    public string DeleteSpeakerLink { get; set; } = null!;
}