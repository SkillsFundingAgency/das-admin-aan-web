using RestEase;
using SFA.DAS.Admin.Aan.Application.OuterApi.Regions;

namespace SFA.DAS.Admin.Aan.Application.Services;

public interface IOuterApiClient
{
    [Get("/regions")]
    Task<GetRegionsResult> GetRegions(CancellationToken cancellationToken);
}
