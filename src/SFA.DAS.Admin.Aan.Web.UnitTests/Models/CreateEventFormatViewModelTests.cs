using AutoFixture.NUnit3;
using FluentAssertions;
using SFA.DAS.Admin.Aan.Application.Constants;
using SFA.DAS.Admin.Aan.Application.Extensions;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvents;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Models;
public class CreateEventFormatViewModelTests
{
    [TestCase(EventFormat.InPerson)]
    [TestCase(EventFormat.Hybrid)]
    [TestCase(EventFormat.Online)]
    [TestCase(null)]
    public void ViewModel_ContainsExpectedValues(EventFormat? eventFormat)
    {
        var expectedEventFormatChecklistLookup = new ChecklistLookup[]
        {
            new(EventFormat.InPerson.GetDescription()!, EventFormat.InPerson.ToString(),
                eventFormat == EventFormat.InPerson),
            new(EventFormat.Online.GetDescription()!, EventFormat.Online.ToString(),
                eventFormat == EventFormat.Online),
            new(EventFormat.Hybrid.GetDescription()!, EventFormat.Hybrid.ToString(),
                eventFormat == EventFormat.Hybrid)
        };

        var vm = new CreateEventFormatViewModel { EventFormat = eventFormat };
        vm.EventFormats.Count.Should().Be(3);
        vm.EventFormat.Should().Be(eventFormat);
        vm.EventFormats.Should().BeEquivalentTo(expectedEventFormatChecklistLookup);
    }

    [Test, AutoData]
    public void Operator_GivenGetCreateEventFormatRequest_ReturnsViewModel(GetCreateEventFormatRequest source)
    {
        CreateEventFormatViewModel sut = source;
        sut.EventFormat.Should().Be(source.EventFormat);
    }
}
