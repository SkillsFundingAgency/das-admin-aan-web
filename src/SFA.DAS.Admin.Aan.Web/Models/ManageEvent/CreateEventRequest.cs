// using Microsoft.AspNetCore.Mvc.Routing;
// using SFA.DAS.Admin.Aan.Application.Constants;
// using static System.Int64;
//
// namespace SFA.DAS.Admin.Aan.Web.Models.ManageEvent;
// public class CreateEventRequest
// {
//     public Guid RequestedByMemberId { get; set; }
//     public int? CalendarTypeId { get; set; }
//     public EventFormat? EventFormat { get; set; }
//     public DateTime? StartDate { get; set; }
//     public DateTime? EndDate { get; set; }
//     public string? Title { get; set; }
//     public string? Description { get; set; }
//     public string? Summary { get; set; }
//     public int? RegionId { get; set; }
//     public string? Location { get; set; }
//     public string? Postcode { get; set; }
//     public double? Latitude { get; set; }
//     public double? Longitude { get; set; }
//     public long? Urn { get; set; }
//     public string? EventLink { get; set; }
//     public string? ContactName { get; set; }
//     public string? ContactEmail { get; set; }
//     public int? PlannedAttendees { get; set; }
//     public List<Guest> Guests { get; set; } = new List<Guest>();
//
//     public static implicit operator CreateEventRequest(EventSessionModel source)
//     {
//         TryParse(source.Urn, out var urn);
//
//         var guestSpeakers = new List<Guest>();
//
//         if (source.HasGuestSpeakers.HasValue && source.HasGuestSpeakers.Value && source.GuestSpeakers.Any())
//         {
//             guestSpeakers.AddRange(source.GuestSpeakers.Select(guest => new Guest(guest.GuestName, guest.GuestJobTitle)));
//         }
//
//         return new()
//         {
//             CalendarTypeId = source.CalendarId,
//             EventFormat = source.EventFormat,
//             Title = source.EventTitle,
//             Description = source.EventOutline,
//             Summary = source.EventSummary,
//             RegionId = source.RegionId,
//             Guests = guestSpeakers,
//             StartDate = source.Start.Value.ToUniversalTime(),
//             EndDate = source?.End.Value.ToUniversalTime(),
//             Location = source?.Location,
//             EventLink = source?.EventLink,
//             Urn = urn,
//             ContactName = source?.ContactName,
//             ContactEmail = source?.ContactEmail,
//             PlannedAttendees = source?.PlannedAttendees,
//             Postcode = source?.Postcode,
//             Latitude = source?.Latitude,
//             Longitude = source?.Longitude
//         };
//     }
// }
//
// public class Guest
// {
//     public Guest(string name, string jobTitle)
//     {
//         GuestName = name;
//         GuestJobTitle = jobTitle;
//     }
//
//     public string GuestName { get; set; }
//     public string GuestJobTitle { get; set; }
// }
//
//
//
