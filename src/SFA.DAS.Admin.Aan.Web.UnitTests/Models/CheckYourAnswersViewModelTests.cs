using AutoFixture.NUnit3;
using FluentAssertions;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Models;

public class CheckYourAnswersViewModelTests
{
    [Test, AutoData]
    public void Operator_GivenEventSessionModel_ReturnsViewModel(EventSessionModel source)
    {
        source.StartHour = 12;
        source.StartMinutes = 25;
        source.EndHour = 13;
        source.EndMinutes = 30;
        CheckYourAnswersViewModel sut = source;
        sut.Should().BeEquivalentTo(source, options => options.ExcludingMissingMembers());
    }
}