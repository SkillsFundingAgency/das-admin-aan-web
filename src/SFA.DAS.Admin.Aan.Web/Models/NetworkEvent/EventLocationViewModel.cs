using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

public class EventLocationViewModel : EventLocationSubmitModel, IEventPageEditFields
{
    public string? EventLocation { get; set; }
    public string? OnlineEventLink { get; set; }

    public string LocationTitle { get; set; } = null!;

    public int EventLocationMaxCount => EventLocationViewModelValidator.EventLocationMaxCount;
    public int EventOnlineEventLinkMaxCount => EventLocationViewModelValidator.EventOnlineLinkMaxLength;
    public string? PageTitle { get; set; }
    public string? PostLink { get; set; }
    public string? CancelLink { get; set; }
}

public class EventLocationSubmitModel
{
    public string? SearchTerm { get; set; }
    public string? OrganisationName { get; set; }
    public string? Town { get; set; }
    public string? County { get; set; }
    public string? Postcode { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public double? Longitude { get; set; }
    public double? Latitude { get; set; }

}