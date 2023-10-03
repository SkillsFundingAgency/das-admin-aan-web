using SFA.DAS.Admin.Aan.Application.Constants;

namespace SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

public class EventSessionModel
{
    public bool HasSeenPreview { get; set; }

    public EventFormat? EventFormat { get; set; }

    public string? EventTitle { get; set; }
    public int? CalendarId { get; set; }
    public int? RegionId { get; set; }

    public string? EventOutline { get; set; }
    public string? EventSummary { get; set; }
    public bool? HasGuestSpeakers { get; set; }
    public List<GuestSpeaker> GuestSpeakers { get; set; } = new List<GuestSpeaker>();

    public DateTime? DateOfEvent { get; set; }
    public int? StartHour { get; set; }
    public int? StartMinutes { get; set; }
    public int? EndHour { get; set; }
    public int? EndMinutes { get; set; }

    public string? Location { get; set; }
    public string? EventLink { get; set; }

    public double? Longitude { get; set; }
    public double? Latitude { get; set; }
    public string? Postcode { get; set; }

    public string? SchoolName { get; set; }
    public string? Urn { get; set; }
    public bool? IsAtSchool { get; set; }

    public string? ContactName { get; set; }
    public string? ContactEmail { get; set; }

    public int? PlannedAttendees { get; set; }

    public DateTime? Start
    {
        get
        {
            if (DateOfEvent == null || !StartHour.HasValue || !StartMinutes.HasValue) return null;

            var dateOfEventToUse = DateOfEvent.Value;

            return new DateTime(dateOfEventToUse.Year, dateOfEventToUse.Month, dateOfEventToUse.Day, StartHour.Value,
                StartMinutes.Value, 0);
        }
    }

    public DateTime? End
    {
        get
        {
            if (DateOfEvent == null || !EndHour.HasValue || !EndMinutes.HasValue) return null;

            var dateOfEventToUse = DateOfEvent.Value;

            return new DateTime(dateOfEventToUse.Year, dateOfEventToUse.Month, dateOfEventToUse.Day, EndHour.Value,
                EndHour.Value, 0);
        }
    }
}
