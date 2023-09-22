using FluentAssertions;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

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
    [TestCase(OrganisationName, null, null, null, null, Postcode, $"{OrganisationName}, {Postcode}")]
    [TestCase(null, Address1, null, null, null, Postcode, $"{Address1}, {Postcode}")]
    [TestCase(null, null, Address2, null, null, Postcode, $"{Address2}, {Postcode}")]
    [TestCase(null, null, null, County, null, Postcode, $"{County}, {Postcode}")]
    [TestCase(null, null, null, null, Town, Postcode, $"{Town}, {Postcode}")]
    [TestCase(null, Address1, Address2, null, null, Postcode, $"{Address1}, {Postcode}")]
    [TestCase(OrganisationName, Address1, null, null, null, Postcode, $"{OrganisationName}, {Address1}, {Postcode}")]
    [TestCase(OrganisationName, Address1, Address2, null, null, Postcode, $"{OrganisationName}, {Address1}, {Postcode}")]
    [TestCase(OrganisationName, Address1, Address2, Town, null, Postcode, $"{OrganisationName}, {Address1}, {Town}, {Postcode}")]
    [TestCase(OrganisationName, Address1, Address2, Town, County, Postcode, $"{OrganisationName}, {Address1}, {Town}, {County}, {Postcode}")]
    [TestCase(OrganisationName, Address1, null, Town, null, Postcode, $"{OrganisationName}, {Address1}, {Town}, {Postcode}")]
    [TestCase(OrganisationName, Address1, null, Town, County, Postcode, $"{OrganisationName}, {Address1}, {Town}, {County}, {Postcode}")]
    [TestCase(null, Address1, null, null, null, Postcode, $"{Address1}, {Postcode}")]
    [TestCase(null, Address1, Address2, null, null, Postcode, $"{Address1}, {Postcode}")]
    [TestCase(null, Address1, Address2, Town, null, Postcode, $"{Address1}, {Town}, {Postcode}")]
    [TestCase(null, Address1, Address2, Town, County, Postcode, $"{Address1}, {Town}, {County}, {Postcode}")]
    [TestCase(null, Address1, null, Town, null, Postcode, $"{Address1}, {Town}, {Postcode}")]
    [TestCase(null, Address1, null, Town, County, Postcode, $"{Address1}, {Town}, {County}, {Postcode}")]
    [TestCase(OrganisationName, null, Address2, null, null, Postcode, $"{OrganisationName}, {Address2}, {Postcode}")]
    [TestCase(OrganisationName, null, Address2, Town, null, Postcode, $"{OrganisationName}, {Address2}, {Town}, {Postcode}")]
    [TestCase(OrganisationName, null, Address2, Town, County, Postcode, $"{OrganisationName}, {Address2}, {Town}, {County}, {Postcode}")]
    [TestCase(OrganisationName, null, Address2, null, County, Postcode, $"{OrganisationName}, {Address2}, {County}, {Postcode}")]
    [TestCase(null, null, Address2, null, null, Postcode, $"{Address2}, {Postcode}")]
    [TestCase(null, null, Address2, Town, null, Postcode, $"{Address2}, {Town}, {Postcode}")]
    [TestCase(null, null, Address2, Town, County, Postcode, $"{Address2}, {Town}, {County}, {Postcode}")]
    [TestCase(null, null, Address2, null, County, Postcode, $"{Address2}, {County}, {Postcode}")]
    [TestCase(null, null, null, Town, null, Postcode, $"{Town}, {Postcode}")]
    [TestCase(null, null, null, Town, County, Postcode, $"{Town}, {County}, {Postcode}")]
    [TestCase(null, null, null, null, County, Postcode, $"{County}, {Postcode}")]
    public void ViewModel_LocationNameIsChecked(string? organisationName, string? address1, string? address2, string? town, string? county, string? postcode, string? locationName)
    {
        var model = new EventLocationViewModel
        {
            OrganisationName = organisationName,
            AddressLine1 = address1,
            AddressLine2 = address2,
            Town = town,
            County = county,
            Postcode = postcode
        };

        model.EventLocation.Should().Be(locationName);
    }



    // [TestCase(EventFormat.InPerson)]
    // [TestCase(EventFormat.Hybrid)]
    // [TestCase(EventFormat.Online)]
    // [TestCase(null)]
    // public void ViewModel_ContainsExpectedValues(EventFormat? eventFormat)
    // {
    //     var expectedEventFormatChecklistLookup = new ChecklistLookup[]
    //     {
    //         new(EventFormat.InPerson.GetDescription(), EventFormat.InPerson.ToString(),
    //             eventFormat == EventFormat.InPerson),
    //         new(EventFormat.Online.GetDescription(), EventFormat.Online.ToString(),
    //             eventFormat == EventFormat.Online),
    //         new(EventFormat.Hybrid.GetDescription(), EventFormat.Hybrid.ToString(),
    //             eventFormat == EventFormat.Hybrid)
    //     };
    //
    //     var vm = new EventFormatViewModel { EventFormat = eventFormat };
    //     vm.EventFormats.Count.Should().Be(3);
    //     vm.EventFormat.Should().Be(eventFormat);
    //     vm.EventFormats.Should().BeEquivalentTo(expectedEventFormatChecklistLookup);
    // }
    //
    // [Test, AutoData]
    // public void Operator_GivenGetEventFormatRequest_ReturnsViewModel(GetManageEventFormatRequest source)
    // {
    //     EventFormatViewModel sut = source;
    //     sut.EventFormat.Should().Be(source.EventFormat);
    // }
}
