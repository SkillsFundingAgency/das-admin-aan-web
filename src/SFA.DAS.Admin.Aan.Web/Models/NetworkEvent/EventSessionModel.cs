using SFA.DAS.Admin.Aan.Application.Constants;

namespace SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

public class EventSessionModel
{
    public EventFormat? EventFormat { get; set; }

    public string? EventTitle { get; set; }
    public int? EventTypeId { get; set; }
    public int? EventRegionId { get; set; }

    public string? EventOutline { get; set; }
    public string? EventSummary { get; set; }
    public bool? HasGuestSpeakers { get; set; }

    public DateTime? DateOfEvent { get; set; }
    public int? StartHour { get; set; }
    public int? StartMinutes { get; set; }
    public int? EndHour { get; set; }
    public int? EndMinutes { get; set; }

    public List<GuestSpeaker> GuestSpeakers { get; set; } = new List<GuestSpeaker>();
}
