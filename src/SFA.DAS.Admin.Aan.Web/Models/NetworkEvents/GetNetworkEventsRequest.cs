using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.Admin.Aan.Web.Models.NetworkEvents;

public class GetNetworkEventsRequest
{
    [FromQuery]
    public int? Page { get; set; }

    [FromQuery]
    public int? PageSize { get; set; }
}
