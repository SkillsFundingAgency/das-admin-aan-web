using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Validators.CreateEvent;

namespace SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

public class CreateEventDescriptionViewModel : IEventPageEditFields
{
    public string? EventOutline { get; set; }
    public string? EventSummary { get; set; }

    public int EventOutlineMaxCount => CreateEventDescriptionViewModelValidator.EventOutlineMaxLength;
    public int EventSummaryMaxCount => CreateEventDescriptionViewModelValidator.EventSummaryMaxLength;

    public string? PageTitle { get; set; }
    public string? PostLink { get; set; }
    public string? CancelLink { get; set; }
}