﻿using RestEase;
using SFA.DAS.Admin.Aan.Application.Constants;
using SFA.DAS.Admin.Aan.Application.OuterApi.Admins;
using SFA.DAS.Admin.Aan.Application.OuterApi.Calendar;
using SFA.DAS.Admin.Aan.Application.OuterApi.Calendar.Responses;
using SFA.DAS.Admin.Aan.Application.OuterApi.Locations;
using SFA.DAS.Admin.Aan.Application.OuterApi.Regions;
using SFA.DAS.Admin.Aan.Application.OuterApi.Schools;
using Calendar = SFA.DAS.Admin.Aan.Application.OuterApi.Calendar.Calendar;

namespace SFA.DAS.Admin.Aan.Application.Services;

public interface IOuterApiClient
{
    [Get("/regions")]
    Task<GetRegionsResult> GetRegions(CancellationToken cancellationToken);

    [Get("/calendars")]
    Task<List<Calendar>> GetCalendars(CancellationToken cancellationToken);

    [Get("calendarEvents")]
    Task<GetCalendarEventsQueryResult> GetCalendarEvents([Header(RequestHeaders.RequestedByMemberIdHeader)] Guid requestedByMemberId, [QueryMap] IDictionary<string, string[]> parameters, CancellationToken cancellationToken);

    [Post("calendarEvents")]
    Task<PostCalendarEventResult> PostCalendarEvent([Header(RequestHeaders.RequestedByMemberIdHeader)] Guid requestedByMemberId, [Body] CreateEventRequest request, CancellationToken cancellationToken);


    [Get("/locations")]
    Task<GetAddressesResult> GetAddresses([Query] string query, CancellationToken cancellationToken);

    [Post("admins")]
    Task<LookupAdminMemberResult> GetAdminMember([Body] LookupAdminMemberRequest request, CancellationToken cancellationToken);

    [Get("/Schools/find/{searchTerm}")]
    Task<GetSchoolsResult> GetSchools([Path("searchTerm")] string searchTerm, CancellationToken cancellationToken);
}
