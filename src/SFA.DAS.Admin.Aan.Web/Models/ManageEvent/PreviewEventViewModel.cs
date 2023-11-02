using SFA.DAS.Admin.Aan.Application.Constants;

namespace SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

public class PreviewEventViewModel
{
    public EventFormat? EventFormat { get; set; }
    public string? EventType { get; set; }

    // public string? PreviewLink { get; set; }
    // public bool HasSeenPreview { get; set; }

    // public string? EventTitle { get; set; }

    // public string? EventRegion { get; set; }
    //
    // public string? EventOutline { get; set; }
    // public string? EventSummary { get; set; }
    // public bool? HasGuestSpeakers { get; set; }
    // public List<GuestSpeaker> GuestSpeakers { get; set; } = new List<GuestSpeaker>();
    //
    // public DateTime? Start { get; set; }
    // public DateTime? End { get; set; }
    //
    // public string? EventLocation { get; set; }
    // public string? OnlineEventLink { get; set; }
    //
    // public string? SchoolName { get; set; }
    //
    // public bool? IsAtSchool { get; set; }
    //
    // public string? OrganiserName { get; set; }
    // public string? OrganiserEmail { get; set; }
    //
    // public int? NumberOfAttendees { get; set; }
    //
    // public string? EventFormatLink { get; set; }
    //
    // public string? EventLocationLink { get; set; }
    //
    // public string? EventTypeLink { get; set; }
    // public string? EventDateTimeLink { get; set; }
    // public string? EventDescriptionLink { get; set; }
    // public string? HasGuestSpeakersLink { get; set; }
    // public string? GuestSpeakersListLink { get; set; }
    // public string? OrganiserDetailsLink { get; set; }
    // public string? IsAtSchoolLink { get; set; }
    // public string? SchoolNameLink { get; set; }
    // public string? NumberOfAttendeesLink { get; set; }


    public static implicit operator PreviewEventViewModel(EventSessionModel source)
        => new()
        {
            EventFormat = source.EventFormat,
            // EventTitle = source.EventTitle,
            // EventOutline = source.EventOutline,
            // EventSummary = source.EventSummary,
            // HasGuestSpeakers = source.HasGuestSpeakers,
            // GuestSpeakers = source.GuestSpeakers,
            // Start = source.Start,
            // End = source.End,
            // EventLocation = source.Location,
            // OnlineEventLink = source.EventLink,
            // SchoolName = source.SchoolName,
            // IsAtSchool = source.IsAtSchool,
            // OrganiserName = source.ContactName,
            // OrganiserEmail = source.ContactEmail,
            // NumberOfAttendees = source.PlannedAttendees
        };
}
