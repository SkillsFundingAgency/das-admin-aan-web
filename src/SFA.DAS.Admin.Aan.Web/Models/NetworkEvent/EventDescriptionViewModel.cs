using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

public class EventDescriptionViewModel : IEventPageEditFields
{
    public string? EventOutline { get; set; }
    public string? EventSummary { get; set; }

    public int EventOutlineMaxCount => EventDescriptionViewModelValidator.EventOutlineMaxLength;
    public int EventSummaryMaxCount => EventDescriptionViewModelValidator.EventSummaryMaxLength;

    public string? PageTitle { get; set; }
    public string? PostLink { get; set; }
    public string? CancelLink { get; set; }
}