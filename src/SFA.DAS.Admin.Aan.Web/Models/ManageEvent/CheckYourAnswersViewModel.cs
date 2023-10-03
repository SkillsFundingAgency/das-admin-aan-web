using SFA.DAS.Admin.Aan.Application.Constants;

namespace SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

public class CheckYourAnswersViewModel : EventPageEditFields
{
    public string PreviewLink { get; set; }
    public bool HasSeenPreview { get; set; }

    public EventFormat? EventFormat { get; set; }

    public string? EventTitle { get; set; }
    // public int? EventTypeId { get; set; }
    // public int? EventRegionId { get; set; }

    public string? EventType { get; set; }
    public string EventRegion { get; set; }

    // public List<RegionSelection> EventRegions { get; set; } = new List<RegionSelection>();
    // public List<EventTypeSelection> EventTypes { get; set; } = new List<EventTypeSelection>();

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

    public string GetDateAndTimeFormatted()
    {
        if (DateOfEvent == null || !StartHour.HasValue || !StartMinutes.HasValue || !EndHour.HasValue || !EndMinutes.HasValue) return "";

        var dateOfEventToUse = DateOfEvent.Value;

        var startDate = new DateTime(dateOfEventToUse.Year, dateOfEventToUse.Month, dateOfEventToUse.Day, StartHour.Value,
            StartMinutes.Value, 0);
        var endDate = new DateTime(dateOfEventToUse.Year, dateOfEventToUse.Month, dateOfEventToUse.Day, EndHour.Value,
            EndMinutes.Value, 0);

        var startDateTimeFormatted = startDate.Minute == 0
            ? startDate.ToString("htt").ToLower()
            : startDate.ToString("h:mmtt").ToLower();

        var endDateTimeFormatted = endDate.Minute == 0
            ? endDate.ToString("htt").ToLower()
            : endDate.ToString("h:mmtt").ToLower();

        return startDate.ToString("d MMMM yyyy") + ", " + startDateTimeFormatted + " to " + endDateTimeFormatted;
    }

    public static implicit operator CheckYourAnswersViewModel(EventSessionModel source)
        => new()
        {
            HasSeenPreview = source.HasSeenPreview,
            EventFormat = source.EventFormat,
            EventTitle = source.EventTitle,
            EventOutline = source.EventOutline,
            EventSummary = source.EventSummary,
            HasGuestSpeakers = source.HasGuestSpeakers,
            GuestSpeakers = source.GuestSpeakers,
            DateOfEvent = source.DateOfEvent,
            StartHour = source.StartHour,
            StartMinutes = source.StartMinutes,
            EndHour = source.EndHour,
            EndMinutes = source.EndMinutes,
            EventLocation = source.EventLocation,
            OnlineEventLink = source.OnlineEventLink,
            SchoolName = source.SchoolName,
            Urn = source.Urn,
            IsAtSchool = source.IsAtSchool,
            OrganiserName = source.OrganiserName,
            OrganiserEmail = source.OrganiserEmail,
            NumberOfAttendees = source.NumberOfAttendees
        };
}