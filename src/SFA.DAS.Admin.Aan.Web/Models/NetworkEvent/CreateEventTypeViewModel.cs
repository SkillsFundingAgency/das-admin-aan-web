using SFA.DAS.Admin.Aan.Web.Infrastructure;

namespace SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

public class CreateEventTypeViewModel : IBackLink
{
    public string? EventTitle { get; set; }
    public string BackLink { get; set; } = null!;

    public int? EventTypeId { get; set; }
    public int? EventRegionId { get; set; }

    // public List<DropdownLookup> EventTypes { get; set; } = new List<DropdownLookup>();
    public List<RegionSelection> EventRegions { get; set; } = new List<RegionSelection>();

    public List<EventType> EventTypes { get; set; } = new List<EventType>();

    // public static implicit operator CreateEventFormatViewModel(GetCreateEventFormatRequest request)
    //     => new()
    //     {
    //         EventFormat = request.EventFormat
    //     };
}

public class RegionSelection
{
    public int RegionId { get; set; }
    public string Name { get; set; }

    public RegionSelection(string name, int id)
    {
        Name = name;
        RegionId = id;
    }
}

public class EventType
{
    public int EventTypeId { get; set; }
    public string Name { get; set; }

    public EventType(string name, int id)
    {
        Name = name;
        EventTypeId = id;
    }
}