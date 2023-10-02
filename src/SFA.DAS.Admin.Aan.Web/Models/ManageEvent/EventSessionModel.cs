using SFA.DAS.Admin.Aan.Application.Constants;

namespace SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

public class EventSessionModel
{
    public bool HasSeenPreview { get; set; }

    public EventFormat? EventFormat { get; set; }

    public string? EventTitle { get; set; }
    public int? EventTypeId { get; set; }
    public int? EventRegionId { get; set; }

    public string? EventOutline { get; set; }
    public string? EventSummary { get; set; }
    public bool? HasGuestSpeakers { get; set; }
    public List<GuestSpeaker> GuestSpeakers { get; set; } = new List<GuestSpeaker>();

    public DateTime? DateOfEvent { get; set; }
    public int? StartHour { get; set; }
    public int? StartMinutes { get; set; }
    public int? EndHour { get; set; }
    public int? EndMinutes { get; set; }

    public string? EventLocation { get; set; }
    public string? OnlineEventLink { get; set; }

    public string? SchoolName { get; set; }
    public string? Urn { get; set; }
    public bool? IsAtSchool { get; set; }

    public string? OrganiserName { get; set; }
    public string? OrganiserEmail { get; set; }

    public int? NumberOfAttendees { get; set; }
}
