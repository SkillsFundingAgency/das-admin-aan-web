using FluentAssertions;
using SFA.DAS.Aan.SharedUi.Constants;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Models;
public class EventLocationViewModelTests
{
    private const string Postcode = "ABC 123";
    private const string OrganisationName = "org 1";
    private const string Address1 = "address line 1";
    private const string Address2 = "address line 2";
    private const string County = "county 1";
    private const string Town = "Town 1";

    [TestCase(null, null, null, null, null, null, null)]
    [TestCase(OrganisationName, null, null, null, null, Postcode, $"{OrganisationName}")]
    [TestCase(null, Address1, null, null, null, Postcode, $"{Address1}")]
    [TestCase(null, null, Address2, null, null, Postcode, $"{Address2}")]
    [TestCase(null, null, null, County, null, Postcode, $"{County}")]
    [TestCase(null, null, null, null, Town, Postcode, $"{Town}")]
    [TestCase(null, Address1, Address2, null, null, Postcode, $"{Address1}")]
    [TestCase(OrganisationName, Address1, null, null, null, Postcode, $"{OrganisationName}, {Address1}")]
    [TestCase(OrganisationName, Address1, Address2, null, null, Postcode, $"{OrganisationName}, {Address1}")]
    [TestCase(OrganisationName, Address1, Address2, Town, null, Postcode, $"{OrganisationName}, {Address1}, {Town}")]
    [TestCase(OrganisationName, Address1, Address2, Town, County, Postcode, $"{OrganisationName}, {Address1}, {Town}, {County}")]
    [TestCase(OrganisationName, Address1, null, Town, null, Postcode, $"{OrganisationName}, {Address1}, {Town}")]
    [TestCase(OrganisationName, Address1, null, Town, County, Postcode, $"{OrganisationName}, {Address1}, {Town}, {County}")]
    [TestCase(null, Address1, null, null, null, Postcode, $"{Address1}")]
    [TestCase(null, Address1, Address2, null, null, Postcode, $"{Address1}")]
    [TestCase(null, Address1, Address2, Town, null, Postcode, $"{Address1}, {Town}")]
    [TestCase(null, Address1, Address2, Town, County, Postcode, $"{Address1}, {Town}, {County}")]
    [TestCase(null, Address1, null, Town, null, Postcode, $"{Address1}, {Town}")]
    [TestCase(null, Address1, null, Town, County, Postcode, $"{Address1}, {Town}, {County}")]
    [TestCase(OrganisationName, null, Address2, null, null, Postcode, $"{OrganisationName}, {Address2}")]
    [TestCase(OrganisationName, null, Address2, Town, null, Postcode, $"{OrganisationName}, {Address2}, {Town}")]
    [TestCase(OrganisationName, null, Address2, Town, County, Postcode, $"{OrganisationName}, {Address2}, {Town}, {County}")]
    [TestCase(OrganisationName, null, Address2, null, County, Postcode, $"{OrganisationName}, {Address2}, {County}")]
    [TestCase(null, null, Address2, null, null, Postcode, $"{Address2}")]
    [TestCase(null, null, Address2, Town, null, Postcode, $"{Address2}, {Town}")]
    [TestCase(null, null, Address2, Town, County, Postcode, $"{Address2}, {Town}, {County}")]
    [TestCase(null, null, Address2, null, County, Postcode, $"{Address2}, {County}")]
    [TestCase(null, null, null, Town, null, Postcode, $"{Town}")]
    [TestCase(null, null, null, Town, County, Postcode, $"{Town}, {County}")]
    [TestCase(null, null, null, null, County, Postcode, $"{County}")]
    public void ViewModel_LocationNameIsChecked(string? organisationName, string? address1, string? address2, string? town, string? county, string? postcode, string? locationName)
    {
        var model = new LocationViewModel
        {
            OrganisationName = organisationName,
            AddressLine1 = address1,
            AddressLine2 = address2,
            Town = town,
            County = county,
            Postcode = postcode,
            Latitude = null,
            Longitude = null
        };

        model.EventLocation.Should().Be(locationName);
    }

    [TestCase(null, false)]
    [TestCase(EventFormat.Hybrid, true)]
    [TestCase(EventFormat.Online, true)]
    [TestCase(EventFormat.InPerson, false)]
    public void ViewModel_CheckShowEventOnlineLink(EventFormat? eventFormat, bool expectedValue)
    {
        var longitude = new Random().NextDouble();
        var latitude = new Random().NextDouble();
        var model = new LocationViewModel { EventFormat = eventFormat, Longitude = longitude, Latitude = latitude };
        model.ShowOnlineEventLink.Should().Be(expectedValue);
        model.Latitude.Should().Be(latitude);
        model.Longitude.Should().Be(longitude);
    }

    [TestCase(null, false)]
    [TestCase(EventFormat.Hybrid, true)]
    [TestCase(EventFormat.Online, false)]
    [TestCase(EventFormat.InPerson, true)]
    public void ViewModel_ShowLocationDropdown(EventFormat? eventFormat, bool expectedValue)
    {
        var model = new LocationViewModel { EventFormat = eventFormat };
        model.ShowLocationDropdown.Should().Be(expectedValue);
    }
}
