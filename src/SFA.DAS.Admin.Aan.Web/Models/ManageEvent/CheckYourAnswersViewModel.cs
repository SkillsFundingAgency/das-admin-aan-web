﻿using SFA.DAS.Admin.Aan.Application.Constants;

namespace SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

public class CheckYourAnswersViewModel : ManageEventViewModelBase
{
    public string? PreviewLink { get; set; }
    public bool HasSeenPreview { get; set; }

    public EventFormat? EventFormat { get; set; }

    public string? EventTitle { get; set; }
    public string? EventType { get; set; }
    public string? EventRegion { get; set; }

    public string? EventOutline { get; set; }
    public string? EventSummary { get; set; }
    public bool? HasGuestSpeakers { get; set; }
    public List<GuestSpeaker> GuestSpeakers { get; set; } = new List<GuestSpeaker>();

    public DateTime? Start { get; set; }
    public DateTime? End { get; set; }

    public string? EventLocation { get; set; }
    public string? OnlineEventLink { get; set; }

    public string? SchoolName { get; set; }

    public bool? IsAtSchool { get; set; }

    public string? OrganiserName { get; set; }
    public string? OrganiserEmail { get; set; }

    public int? NumberOfAttendees { get; set; }

    public bool ShowLocation =>
        EventFormat is Application.Constants.EventFormat.InPerson or Application.Constants.EventFormat.Hybrid;

    public bool ShowOnlineEventLink =>
        EventFormat is Application.Constants.EventFormat.Online or Application.Constants.EventFormat.Hybrid;

    public string GetDateAndTimeFormatted()
    {
        if (Start == null || End == null) return "";

        var startDateTimeFormatted = Start.Value.ToString("h:mmtt").ToLower();

        var endDateTimeFormatted = End.Value.ToString("h:mmtt").ToLower();

        return Start.Value.ToString("d MMMM yyyy") + ", " + startDateTimeFormatted + " to " + endDateTimeFormatted;
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
            Start = source.Start,
            End = source.End,
            EventLocation = source.Location,
            OnlineEventLink = source.EventLink,
            SchoolName = source.SchoolName,
            IsAtSchool = source.IsAtSchool,
            OrganiserName = source.ContactName,
            OrganiserEmail = source.ContactEmail,
            NumberOfAttendees = source.PlannedAttendees
        };
}