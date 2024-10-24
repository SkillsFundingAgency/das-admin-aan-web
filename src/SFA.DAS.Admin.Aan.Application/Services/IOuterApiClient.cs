using RestEase;
using SFA.DAS.Aan.SharedUi.Models.AmbassadorProfile;
using SFA.DAS.Admin.Aan.Application.Constants;
using SFA.DAS.Admin.Aan.Application.OuterApi.Admins;
using SFA.DAS.Admin.Aan.Application.OuterApi.Calendar;
using SFA.DAS.Admin.Aan.Application.OuterApi.Calendar.Responses;
using SFA.DAS.Admin.Aan.Application.OuterApi.CalendarEventAttendees;
using SFA.DAS.Admin.Aan.Application.OuterApi.CalendarEvents;
using SFA.DAS.Admin.Aan.Application.OuterApi.Locations;
using SFA.DAS.Admin.Aan.Application.OuterApi.Members;
using SFA.DAS.Admin.Aan.Application.OuterApi.Members.Responses;
using SFA.DAS.Admin.Aan.Application.OuterApi.NotificationSettings;
using SFA.DAS.Admin.Aan.Application.OuterApi.Profiles;
using SFA.DAS.Admin.Aan.Application.OuterApi.Regions;
using SFA.DAS.Admin.Aan.Application.OuterApi.Schools;

namespace SFA.DAS.Admin.Aan.Application.Services;

public interface IOuterApiClient
{
    [Get("/regions")]
    Task<GetRegionsResult> GetRegions(CancellationToken cancellationToken);

    [Get("/calendars")]
    Task<List<CalendarDetail>> GetCalendars(CancellationToken cancellationToken);

    [Get("calendarEvents")]
    Task<GetCalendarEventsQueryResult> GetCalendarEvents([Header(RequestHeaders.RequestedByMemberIdHeader)] Guid requestedByMemberId, [QueryMap] IDictionary<string, string[]> parameters, CancellationToken cancellationToken);

    [Get("calendarEvents/{calendarEventId}")]
    Task<GetCalendarEventQueryResult> GetCalendarEvent([Header(RequestHeaders.RequestedByMemberIdHeader)] Guid requestedByMemberId, [Path("calendarEventId")] Guid calendarEventId, CancellationToken cancellationToken);

    [Post("calendarEvents")]
    Task<PostCalendarEventResult> PostCalendarEvent([Header(RequestHeaders.RequestedByMemberIdHeader)] Guid requestedByMemberId, [Body] CreateEventRequest request, CancellationToken cancellationToken);

    [Put("/calendarEvents/{calendarEventId}")]
    Task UpdateCalendarEvent(
        [Header(RequestHeaders.RequestedByMemberIdHeader)] Guid requestedByMemberId,
        [Path("calendarEventId")] Guid calendarEventId,
        [Body] UpdateCalendarEventRequest request,
        CancellationToken cancellationToken);

    [Delete("/calendarEvents/{calendarEventId}")]
    Task DeleteCalendarEvent(
        [Header(RequestHeaders.RequestedByMemberIdHeader)] Guid requestedByMemberId,
        [Path("calendarEventId")] Guid calendarEventId,
        CancellationToken cancellationToken);

    [Get("/locations")]
    Task<GetAddressesResult> GetAddresses([Query] string query, CancellationToken cancellationToken);

    [Get("/locations/search")]
    Task<GetLocationsBySearchApiResponse> GetLocationsBySearch([Query] string query, CancellationToken cancellationToken);

    [Post("admins")]
    Task<LookupAdminMemberResult> GetAdminMember([Body] LookupAdminMemberRequest request, CancellationToken cancellationToken);

    [Get("/Schools/find/{searchTerm}")]
    Task<GetSchoolsResult> GetSchools([Path("searchTerm")] string searchTerm, CancellationToken cancellationToken);

    [Get("members/")]
    Task<GetMembersResponse> GetMembers([QueryMap] IDictionary<string, string[]> parameters, CancellationToken cancellationToken);

    [Get("/members/{memberId}/profile?public=false")]
    Task<MemberProfileResponse> GetMemberProfile([Path] Guid memberId, [Header(RequestHeaders.RequestedByMemberIdHeader)] Guid requestedByMemberId, CancellationToken cancellationToken);

    [Get("/profiles/{userType}")]
    Task<GetProfilesResult> GetProfilesByUserType([Path] MemberUserType userType, CancellationToken? cancellationToken);

    [Post("/members/{memberId}/leaving")]
    Task<string> PostMemberLeaving([Path] Guid memberId, [Header(RequestHeaders.RequestedByMemberIdHeader)] Guid requestedByMemberId, [Body] PostMemberStatusModel postMemberStatusModel, CancellationToken cancellationToken);

    [Get("notification-settings")]
    Task<GetNotificationSettingsResponse> GetNotificationSettings([Query] Guid memberId, CancellationToken cancellationToken);

    [Post("notification-settings")]
    Task PostNotificationSettings([Query] Guid memberId, [Body] PostNotificationSettings postRequest, CancellationToken cancellationToken);

    [Get("calendarEvents/{calendarEventId}/attendees")]
    Task<GetCalendarEventAttendeesResponse> GetCalendarEventAttendees([Header(RequestHeaders.RequestedByMemberIdHeader)] Guid requestedByMemberId, [Path("calendarEventId")] Guid calendarEventId, CancellationToken cancellationToken);
}
