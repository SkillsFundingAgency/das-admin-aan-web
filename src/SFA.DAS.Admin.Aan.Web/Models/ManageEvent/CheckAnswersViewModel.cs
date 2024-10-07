using SFA.DAS.Aan.SharedUi.Constants;
using SFA.DAS.Aan.SharedUi.Extensions;

namespace SFA.DAS.Admin.Aan.Web.Models.ManageEvent
{
    public class CheckAnswersViewModel : ManageEventViewModelBase
    {
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

        public static implicit operator CheckAnswersViewModel(EventSessionModel source)
            => new()
            {
                HasSeenPreview = source.HasSeenPreview,
                EventFormat = source.EventFormat,
                EventTitle = source.EventTitle,
                EventOutline = source.EventOutline,
                EventSummary = source.EventSummary,
                HasGuestSpeakers = source.HasGuestSpeakers,
                GuestSpeakers = source.GuestSpeakers,
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
    }
}
