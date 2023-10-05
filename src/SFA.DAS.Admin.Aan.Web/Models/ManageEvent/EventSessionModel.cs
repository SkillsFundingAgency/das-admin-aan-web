using SFA.DAS.Admin.Aan.Application.Constants;
using SFA.DAS.Admin.Aan.Application.OuterApi.CalendarEvents;

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
                StartMinutes.Value, 0, DateTimeKind.Unspecified);
        }
    }

    public DateTime? End
    {
        get
        {
            if (DateOfEvent == null || !EndHour.HasValue || !EndMinutes.HasValue) return null;

            var dateOfEventToUse = DateOfEvent.Value;

            return new DateTime(dateOfEventToUse.Year, dateOfEventToUse.Month, dateOfEventToUse.Day, EndHour.Value,
                EndMinutes.Value, 0, DateTimeKind.Unspecified);
        }
    }


    public static implicit operator CreateEventRequest(EventSessionModel source)
    {
        var urnToUse = (long?)null;

        if (long.TryParse(source.Urn, out var urn)
            && source.IsAtSchool.HasValue
            && source.IsAtSchool.Value)
        {
            urnToUse = urn;
        }

        var guestSpeakers = new List<Guest>();

        if (source.HasGuestSpeakers.HasValue
            && source.HasGuestSpeakers.Value
            && source.GuestSpeakers.Any())
        {
            guestSpeakers.AddRange(source.GuestSpeakers.Select(guest => new Guest(guest.GuestName, guest.GuestJobTitle)));
        }

        DateTime? startDate = null;
        if (source.Start.HasValue)
        {
            startDate = source.Start.Value.ToUniversalTime();
        }

        DateTime? endDate = null;
        if (source.End.HasValue)
        {
            endDate = source.End.Value.ToUniversalTime();
        }

        var location = source.Location;
        var postcode = source.Postcode;
        var eventLink = source.EventLink;
        var latitude = source.Latitude;
        var longitude = source.Longitude;

        if (source.EventFormat == Application.Constants.EventFormat.Online)
        {
            location = null;
            postcode = null;
            latitude = null;
            longitude = null;
        }

        if (source.EventFormat == Application.Constants.EventFormat.InPerson)
        {
            eventLink = null;
        }

        return new()
        {
            CalendarId = source.CalendarId,
            EventFormat = source.EventFormat,
            Title = source.EventTitle,
            Description = source.EventOutline,
            Summary = source.EventSummary,
            RegionId = source.RegionId,
            Guests = guestSpeakers,
            StartDate = startDate,
            EndDate = endDate,
            Location = location,
            EventLink = eventLink,
            Urn = urnToUse,
            ContactName = source.ContactName,
            ContactEmail = source.ContactEmail,
            PlannedAttendees = source.PlannedAttendees,
            Postcode = postcode,
            Latitude = latitude,
            Longitude = longitude
        };
    }
}
