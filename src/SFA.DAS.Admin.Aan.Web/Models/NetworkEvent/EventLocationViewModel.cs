using SFA.DAS.Admin.Aan.Web.Infrastructure;

namespace SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

public class EventLocationViewModel : EventLocationSubmitModel, IEventPageEditFields
{
    public string? EventLocation => BuildLocationName();

    public string? OnlineEventLink { get; set; }

    public string LocationTitle { get; set; } = null!;

    public string? PageTitle { get; set; }
    public string? PostLink { get; set; }
    public string? CancelLink { get; set; }

    private string? BuildLocationName()
    {
        if (string.IsNullOrEmpty(Postcode))
            return null;

        var locationDetails = new List<string>();

        if (!string.IsNullOrWhiteSpace(OrganisationName)) locationDetails.Add(OrganisationName);
        if (!string.IsNullOrWhiteSpace(AddressLine1)) locationDetails.Add(AddressLine1);
        if (!string.IsNullOrWhiteSpace(AddressLine2)) locationDetails.Add(AddressLine2);
        if (!string.IsNullOrWhiteSpace(County)) locationDetails.Add(County);
        if (!string.IsNullOrWhiteSpace(Town)) locationDetails.Add(Town);
        if (!string.IsNullOrWhiteSpace(Postcode)) locationDetails.Add(Postcode);

        return string.Join(", ", locationDetails);
    }
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


