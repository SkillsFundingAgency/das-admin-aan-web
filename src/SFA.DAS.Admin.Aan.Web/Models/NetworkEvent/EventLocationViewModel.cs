using SFA.DAS.Admin.Aan.Web.Infrastructure;

namespace SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

public class EventLocationViewModel : EventLocationDetailsModel, IEventPageEditFields
{
    public string? OnlineEventLink { get; set; }

    public string LocationTitle { get; set; } = null!;

    public string? SearchResult { get; set; }
    public string? EventLocation => BuildLocationName();
    public string? PageTitle { get; set; }
    public string? PostLink { get; set; }
    public string? CancelLink { get; set; }
    public int EventOnlineLinkMaxLength => 1000;
    private string? BuildLocationName()
    {
        if (string.IsNullOrEmpty(Postcode)) return null;

        var locationDetails = new List<string>();

        if (!string.IsNullOrWhiteSpace(OrganisationName)) locationDetails.Add(OrganisationName);
        if (!string.IsNullOrWhiteSpace(AddressLine1)) locationDetails.Add(AddressLine1);
        if (!string.IsNullOrWhiteSpace(AddressLine2) && string.IsNullOrEmpty(AddressLine1))
            locationDetails.Add(AddressLine2);
        if (!string.IsNullOrWhiteSpace(Town)) locationDetails.Add(Town);
        if (!string.IsNullOrWhiteSpace(County)) locationDetails.Add(County);
        if (!string.IsNullOrWhiteSpace(Postcode)) locationDetails.Add(Postcode);

        return string.Join(", ", locationDetails);
    }
}

public class EventLocationDetailsModel
{
    public string? SearchTerm { get; set; }
    public string? OrganisationName { get; set; }
    public string? Town { get; set; }
    public string? County { get; set; }
    public string? Postcode { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
}


