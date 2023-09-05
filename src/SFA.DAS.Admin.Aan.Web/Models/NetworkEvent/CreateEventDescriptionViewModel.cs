using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Validators.CreateEvent;

namespace SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

public class CreateEventDescriptionViewModel : IBackLink
{
    public string BackLink { get; set; } = null!;

    public string? EventOutline { get; set; }
    public string? EventSummary { get; set; }

    public bool? GuestSpeaker { get; set; }


    public int EventOutlineMaxCount = CreateEventDescriptionViewModelValidator.EventOutlineMaxLength;
    public int EventSummaryMaxCount = CreateEventDescriptionViewModelValidator.EventSummaryMaxLength;
}