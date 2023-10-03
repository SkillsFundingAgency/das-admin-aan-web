using SFA.DAS.Admin.Aan.Application.Constants;

namespace SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

public class LocationViewModel : ManageEventViewModelBase
{
    public EventFormat? EventFormat { get; set; }
    public string? OnlineEventLink { get; set; }
    public string? SearchResult { get; set; }
    public string? EventLocation => BuildLocationName();
    public int EventOnlineLinkMaxLength => ManageEventValidation.EventOnlineLinkMaxLength;

    public bool ShowLocationDropdown =>
        EventFormat is Application.Constants.EventFormat.InPerson or Application.Constants.EventFormat.Hybrid;

    public bool ShowOnlineEventLink =>
        EventFormat is Application.Constants.EventFormat.Online or Application.Constants.EventFormat.Hybrid;

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


