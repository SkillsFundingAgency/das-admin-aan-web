using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.Admin.Aan.Web.Models.NetworkEvents;

public class GetNetworkEventsRequest
{
    [FromQuery]
    public DateTime? FromDate { get; set; }

    [FromQuery]
    public DateTime? ToDate { get; set; }

    [FromQuery]
    public List<bool> IsActive { get; set; } = new List<bool>();

    [FromQuery]
    public List<int> CalendarId { get; set; } = new List<int>();

    [FromQuery]
    public List<int> RegionId { get; set; } = new List<int>();

    [FromQuery]
    public int? Page { get; set; }

    [FromQuery]
    public int? PageSize { get; set; }
}
