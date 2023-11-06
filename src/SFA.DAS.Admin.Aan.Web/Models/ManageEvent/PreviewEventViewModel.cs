// using SFA.DAS.Admin.Aan.Application.Constants;
// using SFA.DAS.Admin.Aan.Web.Extensions;
//
// namespace SFA.DAS.Admin.Aan.Web.Models.ManageEvent;
//
// public class PreviewEventViewModel
// {
//     public EventFormat? EventFormat { get; set; }
//     public string? EventType { get; set; }
//
//     // public string? PreviewLink { get; set; }
//     // public bool HasSeenPreview { get; set; }
//
//     public string EventTitle { get; set; } = null!;
//
//     public string? Location { get; set; }
//     // public string? EventLink { get; set; } -- not displayed in preview
//
//     // public double? Longitude { get; set; }
//     // public double? Latitude { get; set; }
//     public string? Postcode { get; set; }
//
//
//     // public string? EventRegion { get; set; }
//     //
//     //public string? EventOutline { get; set; }
//     public string EventSummary { get; set; }
//     // public bool? HasGuestSpeakers { get; set; }
//     // public List<GuestSpeaker> GuestSpeakers { get; set; } = new List<GuestSpeaker>();
//     //
//     public DateTime Start { get; set; }
//     public DateTime End { get; set; }
//     public string StartDate { get; init; } = null!;
//
//     public string StartTime { get; init; }
//     public string EndTime { get; init; }
//
//     public List<GuestSpeaker> EventGuests { get; set; } = new List<GuestSpeaker>();
//
//     //
//     // public string? EventLocation { get; set; }
//     // public string? OnlineEventLink { get; set; }
//     //
//     // public string? SchoolName { get; set; }
//     //
//     // public bool? IsAtSchool { get; set; }
//     //
//     public string? ContactName { get; set; }
//     public string ContactEmail { get; set; } = null!;
//
//     public string EmailLink => $"mailto:{ContactEmail}?subject={Uri.EscapeDataString(EventTitle)}";
//
//
//     public int AttendeeCount => 0;
//     public string GoogleMapsLink => Location == null ? string.Empty : $"https://www.google.com/maps/dir//{Location}+{Postcode}";
//
//     public string CheckYourAnswersLink { get; set; } = null!;
//     public string PartialViewName => GetPartialViewName();
//
//
//     //
//     // public int? NumberOfAttendees { get; set; }
//
//     private string GetPartialViewName()
//     {
//         return EventFormat switch
//         {
//             Application.Constants.EventFormat.Online => "_OnlineEventPartial.cshtml",
//             Application.Constants.EventFormat.InPerson => "_InPersonEventPartial.cshtml",
//             Application.Constants.EventFormat.Hybrid => "_HybridEventPartial.cshtml",
//             _ => throw new NotImplementedException($"Failed to find a matching partial view for event format \"{EventFormat}\""),
//         };
//     }
//
//     public static implicit operator PreviewEventViewModel(EventSessionModel source)
//         => new()
//         {
//             EventFormat = source.EventFormat,
//             EventTitle = source.EventTitle!,
//             //EventOutline = source.EventOutline,
//             EventSummary = source.EventSummary!,
//             // HasGuestSpeakers = source.HasGuestSpeakers,
//             EventGuests = source.GuestSpeakers,
//             Start = (DateTime)source.Start!,
//             End = (DateTime)source.End!,
//             StartDate = ((DateTime)source.Start).UtcToLocalTime().ToString("dddd, d MMMM yyyy"),
//             StartTime = ((DateTime)source.Start).UtcToLocalTime().ToString("h:mm tt").ToLower(),
//             EndTime = ((DateTime)source.End).UtcToLocalTime().ToString("h:mm tt").ToLower(),
//             Location = source.Location,
//             Postcode = source.Postcode,
//             // OnlineEventLink = source.EventLink,
//             // SchoolName = source.SchoolName,
//             // IsAtSchool = source.IsAtSchool,
//             ContactName = source.ContactName,
//             ContactEmail = source.ContactEmail!
//         };
// }
