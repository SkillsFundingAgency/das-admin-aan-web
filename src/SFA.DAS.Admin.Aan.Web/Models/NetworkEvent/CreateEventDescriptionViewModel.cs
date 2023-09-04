using SFA.DAS.Admin.Aan.Web.Infrastructure;

namespace SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

public class CreateEventDescriptionViewModel : IBackLink
{
    public string BackLink { get; set; } = null!;

    public string? EventOutline { get; set; }
    public string? EventSummary { get; set; }
}