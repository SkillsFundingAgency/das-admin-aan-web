using AutoFixture.NUnit3;
using FluentAssertions;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Models;
public class RegionSelectionTests
{
    [Test, AutoData]
    public void RegionSelectionConstructor_SetsUpNamdAndId(string name, int id)
    {
        var regionSelection = new RegionSelection(name, id);
        regionSelection.Name.Should().Be(name);
        regionSelection.RegionId.Should().Be(id);
    }
}