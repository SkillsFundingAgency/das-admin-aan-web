using SFA.DAS.Aan.SharedUi.Constants;
using SFA.DAS.Aan.SharedUi.Models;
using SFA.DAS.Aan.SharedUi.OuterApi.Responses;
using SFA.DAS.Admin.Aan.Application.Constants;
using SFA.DAS.Admin.Aan.Application.OuterApi.Calendar.Responses;
using SFA.DAS.Admin.Aan.Application.OuterApi.CalendarEvents;

namespace SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

public class EventSessionModel
{
    public const string NationalRegionName = "National";

    public bool HasSeenPreview { get; set; }
    public bool IsDirectCallFromCheckYourAnswers { get; set; }
    public bool HasChangedEvent { get; set; }
    public EventFormat? EventFormat { get; set; }
    public string? EventTitle { get; set; }
    public int? CalendarId { get; set; }
    public string CalendarName { get; set; } = string.Empty;

    public int? RegionId { get; set; }
    public string? RegionName { get; set; }
    public string? EventOutline { get; set; }
    public string? EventSummary { get; set; }
    public bool? HasGuestSpeakers { get; set; }
    public List<GuestSpeaker> GuestSpeakers { get; set; } = [];

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
    public DateTime? CreatedDate { get; set; }

    public DateTime? LastUpdatedDate { get; set; }

    public Guid? CalendarEventId { get; set; }
    public bool IsAlreadyPublished { get; set; }
    public bool IsActive { get; set; }
    public IEnumerable<AttendeeModel> Attendees { get; set; } = new List<AttendeeModel>();
    public IEnumerable<CancelledAttendeeModel> CancelledAttendees { get; set; } = new List<CancelledAttendeeModel>();

    public string PageTitle => IsAlreadyPublished ? UpdateEvent.PageTitle : CreateEvent.PageTitle;

    public DateTime? Start { get; set; }

    public DateTime? End { get; set; }

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
            guestSpeakers.AddRange(
                source.GuestSpeakers.Select(guest => new Guest(guest.GuestName, guest.GuestJobTitle)));
        }

        var location = source.Location;
        var postcode = source.Postcode;
        var eventLink = source.EventLink;
        var latitude = source.Latitude;
        var longitude = source.Longitude;

        if (source.EventFormat == DAS.Aan.SharedUi.Constants.EventFormat.Online)
        {
            location = null;
            postcode = null;
            latitude = null;
            longitude = null;
        }

        if (source.EventFormat == DAS.Aan.SharedUi.Constants.EventFormat.InPerson)
        {
            eventLink = null;
        }

        return new()
        {
            CalendarId = source.CalendarId,
            EventFormat = source.EventFormat,
            Title = source.EventTitle,
            Summary = source.EventOutline,
            Description = source.EventSummary,
            RegionId = source.RegionId,
            Guests = guestSpeakers,
            StartDate = source.Start,
            EndDate = source.End,
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

    public static implicit operator UpdateCalendarEventRequest(EventSessionModel source)
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
            guestSpeakers.AddRange(
                source.GuestSpeakers.Select(guest => new Guest(guest.GuestName, guest.GuestJobTitle)));
        }

        var location = source.Location;
        var postcode = source.Postcode;
        var eventLink = source.EventLink;
        var latitude = source.Latitude;
        var longitude = source.Longitude;

        if (source.EventFormat == DAS.Aan.SharedUi.Constants.EventFormat.Online)
        {
            location = null;
            postcode = null;
            latitude = null;
            longitude = null;
        }

        if (source.EventFormat == DAS.Aan.SharedUi.Constants.EventFormat.InPerson)
        {
            eventLink = null;
        }

        return new()
        {
            CalendarId = source.CalendarId,
            EventFormat = source.EventFormat,
            Title = source.EventTitle,
            Summary = source.EventOutline,
            Description = source.EventSummary,
            RegionId = source.RegionId,
            Guests = guestSpeakers,
            StartDate = source.Start,
            EndDate = source.End,
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

    public static implicit operator NetworkEventDetailsViewModel(EventSessionModel source)
    {
        var attendees = new List<Attendee>();
        if (source.Attendees.Any())
        {
            attendees = source.Attendees.Select(att =>
                new Attendee(att.MemberId, att.UserType, att.MemberName, att.AddedDate, string.Empty)).ToList();
        }

        var model = new NetworkEventDetailsViewModel(
            source!.CalendarName,
            source.Start.GetValueOrDefault(),
            source.End.GetValueOrDefault(),
            source.EventTitle!,
            source.EventSummary!,
            source.ContactName!,
            source.ContactEmail!)
        {
            EventFormat = source.EventFormat.GetValueOrDefault(),
            LocationDetails = new LocationDetails(source.Location, source.Postcode, source.Latitude, source.Longitude, null),
            EventGuests = source.GuestSpeakers.Select(guest => new EventGuest(guest.GuestName, guest.GuestJobTitle)).ToList(),
            Attendees = attendees,
            IsPreview = true,
            IsActive = source.IsActive
        };

        return model;
    }

    public static implicit operator EventSessionModel(GetCalendarEventQueryResult source)
    {
        var eventFormat = new EventFormat();

        if (source.EventFormat == DAS.Aan.SharedUi.Constants.EventFormat.Hybrid.ToString())
            eventFormat = DAS.Aan.SharedUi.Constants.EventFormat.Hybrid;

        if (source.EventFormat == DAS.Aan.SharedUi.Constants.EventFormat.InPerson.ToString())
            eventFormat = DAS.Aan.SharedUi.Constants.EventFormat.InPerson;

        if (source.EventFormat == DAS.Aan.SharedUi.Constants.EventFormat.Online.ToString())
            eventFormat = DAS.Aan.SharedUi.Constants.EventFormat.Online;

        var guestSpeakers = new List<GuestSpeaker>();

        var i = 1;
        foreach (var guest in source.EventGuests)
        {
            guestSpeakers.Add(new GuestSpeaker(guest.GuestName, guest.GuestJobTitle, i));
            i++;
        }

        var regionName = source.RegionId == null ? NationalRegionName : source.RegionName;

        var model = new EventSessionModel
        {
            EventFormat = eventFormat,
            EventTitle = source.Title,
            CalendarId = source.CalendarId,
            CalendarName = source.CalendarName!,
            RegionId = source.RegionId,
            EventOutline = source.Summary,
            EventSummary = source.Description,
            HasGuestSpeakers = source.EventGuests.Any(),
            GuestSpeakers = guestSpeakers,
            Start = source.StartDate,
            End = source.EndDate,
            Location = source.Location,
            EventLink = source.EventLink,
            Longitude = source.Longitude,
            Latitude = source.Latitude,
            Postcode = source.Postcode,
            SchoolName = source.SchoolName,
            Urn = source.Urn,
            IsAtSchool = !string.IsNullOrEmpty(source.Urn),
            ContactName = source.ContactName,
            ContactEmail = source.ContactEmail,
            PlannedAttendees = source.PlannedAttendees,
            CreatedDate = source.CreatedDate,
            LastUpdatedDate = source.LastUpdatedDate,
            RegionName = regionName,
            CalendarEventId = source.CalendarEventId,
            IsDirectCallFromCheckYourAnswers = true,
            Attendees = source.Attendees,
            CancelledAttendees = source.CancelledAttendees,
            IsActive = source.IsActive
        };

        return model;
    }
}
