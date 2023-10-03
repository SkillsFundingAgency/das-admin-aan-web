using AutoFixture.NUnit3;
using FluentAssertions;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Models;

public class EventTypeSelectionTests
{
    [Test, AutoData]
    public void EventTypeSelectionConstructor_SetsUpNamdAndId(string name, int id)
    {
        var regionSelection = new EventTypeSelection(name, id);
        regionSelection.Name.Should().Be(name);
        regionSelection.EventTypeId.Should().Be(id);
    }
}