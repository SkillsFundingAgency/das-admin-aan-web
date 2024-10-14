using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.Admin.Aan.Web.Models.NetworkEvents;

public class GetNetworkEventsRequest
{
    [FromQuery]
    public DateTime? FromDate { get; set; }

    [FromQuery]
    public DateTime? ToDate { get; set; }

    [FromQuery]
    public List<bool> IsActive { get; set; } = [];

    [FromQuery]
    public List<int> CalendarId { get; set; } = [];

    [FromQuery]
    public List<int> RegionId { get; set; } = [];

    [FromQuery]
    public int? Page { get; set; }

    [FromQuery]
    public int? PageSize { get; set; }

    [FromQuery]
    public List<bool> ShowUserEventsOnly { get; set; } = [];

    [FromQuery] public string? Location { get; set; } = "";

    [FromQuery] public int? Radius { get; set; }

}
