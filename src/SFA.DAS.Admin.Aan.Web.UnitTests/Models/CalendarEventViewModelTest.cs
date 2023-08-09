using AutoFixture.NUnit3;
using FluentAssertions;
using SFA.DAS.Admin.Aan.Domain.Constants;
using SFA.DAS.Admin.Aan.Domain.OuterApi.Responses;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvents;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Models;
public class CalendarEventViewModelTest
{
    [Test, AutoData]
    public void Operator_GivenCalendarEventSummary_ReturnsViewModel(CalendarEventSummary source)
    {
        CalendarEventViewModel sut = source;
        sut.Should().BeEquivalentTo(source, options => options.ExcludingMissingMembers());
        sut.Status.Should().Be(source.IsActive ? EventStatus.Published : EventStatus.Cancelled);
    }

    [TestCase(true, "Published")]
    [TestCase(false, "Cancelled")]
    public void Operator_GivenCalendarEventSummaryIsActive_MatchesStatus(bool isActive, string status)
    {
        var sut = new CalendarEventViewModel { IsActive = isActive };
        sut.Status.Should().Be(status);
    }
}
