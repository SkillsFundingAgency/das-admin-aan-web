using SFA.DAS.Admin.Aan.Application.Constants;

namespace SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

public class CreateEventSessionModel
{
    public EventFormat? EventFormat { get; set; }

    public string? EventTitle { get; set; }
    public int? EventTypeId { get; set; }
    public int? EventRegionId { get; set; }

    public string? EventOutline { get; set; }
    public string? EventSummary { get; set; }
    public bool? GuestSpeaker { get; set; }

}
