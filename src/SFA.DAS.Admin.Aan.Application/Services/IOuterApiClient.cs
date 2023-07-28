using RestEase;
using SFA.DAS.Admin.Aan.Application.OuterApi.Regions;
using SFA.DAS.Admin.Aan.Domain.OuterApi.Responses;

namespace SFA.DAS.Admin.Aan.Application.Services;

public interface IOuterApiClient
{
    [Get("/regions")]
    Task<GetRegionsResult> GetRegions(CancellationToken cancellationToken);

    [Get("calendarEvents")]
    Task<GetCalendarEventsQueryResult> GetCalendarEvents([Header(Domain.Constants.RequestHeaders.RequestedByMemberIdHeader)] Guid requestedByMemberId, [QueryMap] IDictionary<string, string[]> parameters, CancellationToken cancellationToken);

}
