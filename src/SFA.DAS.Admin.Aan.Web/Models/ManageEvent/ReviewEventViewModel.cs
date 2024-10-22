using SFA.DAS.Aan.SharedUi.Constants;
using SFA.DAS.Aan.SharedUi.Extensions;
using System.ComponentModel.DataAnnotations;
using SFA.DAS.Admin.Aan.Application.OuterApi.Calendar.Responses;

namespace SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

public class ReviewEventViewModel : ManageEventViewModelBase
{
    public enum AttendeeSortOrderOption : byte
    {
        [Display(Name="Signed-up date (newest)")]
        SignedUpDescending,
        [Display(Name="Signed-up (oldest)")]
        SignedUpAscending,
        [Display(Name="Surname (A to Z)")]
        SurnameAsc,
        [Display(Name="Surname (Z to A)")]
        SurnameDesc
    }

    public AttendeeSortOrderOption SortOrder { get; set; }

    public string? PreviewLink { get; set; }
    public bool HasSeenPreview { get; set; }
    public bool HasChangedEvent { get; set; }
    public EventFormat? EventFormat { get; set; }
    public string? EventTitle { get; set; }
    public string? EventType { get; set; }
    public string? EventRegion { get; set; }
    public string? EventOutline { get; set; }
    public string? EventSummary { get; set; }
    public bool? HasGuestSpeakers { get; set; }
    public List<GuestSpeaker> GuestSpeakers { get; set; } = [];
    public List<Attendee> Attendees { get; set; } = [];
    public List<CancelledAttendee> CancelledAttendees { get; set; } = [];
    public DateTime? Start { get; set; }
    public DateTime? End { get; set; }
    public string? EventLocation { get; set; }
    public string? OnlineEventLink { get; set; }
    public string? SchoolName { get; set; }
    public bool? IsAtSchool { get; set; }
    public string? OrganiserName { get; set; }
    public string? OrganiserEmail { get; set; }
    public int? NumberOfAttendees { get; set; }
    public string? EventFormatLink { get; set; }
    public string? EventLocationLink { get; set; }
    public string? EventTypeLink { get; set; }
    public string? EventDateTimeLink { get; set; }
    public string? EventDescriptionLink { get; set; }
    public string? HasGuestSpeakersLink { get; set; }
    public string? GuestSpeakersListLink { get; set; }
    public string? OrganiserDetailsLink { get; set; }
    public string? IsAtSchoolLink { get; set; }
    public string? SchoolNameLink { get; set; }
    public string? NumberOfAttendeesLink { get; set; }
    public string? DownloadAttendeesLink { get; set; }
    public DateTime? LastUpdatedDate { get; set; }
    public bool ShowLocation =>
        EventFormat is DAS.Aan.SharedUi.Constants.EventFormat.InPerson or DAS.Aan.SharedUi.Constants.EventFormat.Hybrid;

    public bool ShowOnlineEventLink =>
        EventFormat is DAS.Aan.SharedUi.Constants.EventFormat.Online or DAS.Aan.SharedUi.Constants.EventFormat.Hybrid;

    public string GetDateAndTimeFormatted()
    {
        if (Start == null || End == null) return "";

        var startDateTimeFormatted = Start.Value.ToString("h:mmtt").ToLower();

        var endDateTimeFormatted = End.Value.ToString("h:mmtt").ToLower();

        return Start.Value.ToString("d MMMM yyyy") + ", " + startDateTimeFormatted + " to " + endDateTimeFormatted;
    }

    public static implicit operator ReviewEventViewModel(EventSessionModel source)
        => new()
        {
            HasSeenPreview = source.HasSeenPreview,
            EventFormat = source.EventFormat,
            EventTitle = source.EventTitle,
            EventOutline = source.EventOutline,
            EventSummary = source.EventSummary,
            HasGuestSpeakers = source.HasGuestSpeakers,
            GuestSpeakers = source.GuestSpeakers,
            Attendees = source?.Attendees?.Select(x => new Attendee(x.MemberId, x.MemberName, x.Surname, x.Email, x.AddedDate))
                .OrderByDescending(a => a.SignUpDate)
                .ToList() ?? [],
            CancelledAttendees = source?.CancelledAttendees?.Select(x => new CancelledAttendee(x.MemberId, x.MemberName, x.Email, x.CancelledDate))
                .OrderByDescending(a => a.CancellationDate)
                .ToList() ?? [],
            Start = source.Start?.UtcToLocalTime(),
            End = source.End?.UtcToLocalTime(),
            EventLocation = source.Location + (!string.IsNullOrEmpty(source.Postcode) ? $", {source.Postcode}" : string.Empty),
            OnlineEventLink = source.EventLink,
            SchoolName = source.SchoolName,
            IsAtSchool = source.IsAtSchool,
            OrganiserName = source.ContactName,
            OrganiserEmail = source.ContactEmail,
            NumberOfAttendees = source.PlannedAttendees,
            LastUpdatedDate = source.LastUpdatedDate?.UtcToLocalTime(),
            PageTitle = source.PageTitle,
            HasChangedEvent = source.HasChangedEvent
        };
    
    public record Attendee(Guid Id, string Name, string Surname, string Email, DateTime? SignUpDate);

    public record CancelledAttendee(Guid Id, string Name, string Email, DateTime? CancellationDate);
}