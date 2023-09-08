using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Validators.CreateEvent;

namespace SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

public class CreateEventDescriptionViewModel : IEventPageEditFields
{
    public string? EventOutline { get; set; }
    public string? EventSummary { get; set; }

    public bool? GuestSpeaker { get; set; }


    public int EventOutlineMaxCount => CreateEventDescriptionViewModelValidator.EventOutlineMaxLength;
    public int EventSummaryMaxCount => CreateEventDescriptionViewModelValidator.EventSummaryMaxLength;
    public string PageTitle { get; set; } = null!;
    public string PostLink { get; set; } = null!;
    public string CancelLink { get; set; } = null!;
}