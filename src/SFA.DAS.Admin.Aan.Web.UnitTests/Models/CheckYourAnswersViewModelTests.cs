using AutoFixture.NUnit3;
using FluentAssertions;
using SFA.DAS.Admin.Aan.Application.Constants;
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

    [TestCase(EventFormat.InPerson, true, false)]
    [TestCase(EventFormat.Hybrid, true, true)]
    [TestCase(EventFormat.Online, false, true)]
    public void ViewModel_ShowLocationCheck(EventFormat eventFormat, bool expectedShowLocation, bool expectedShowEventLink)
    {
        var vm = new CheckYourAnswersViewModel { EventFormat = eventFormat };
        vm.ShowLocation.Should().Be(expectedShowLocation);
        vm.ShowOnlineEventLink.Should().Be(expectedShowEventLink);
    }

    [TestCase(null, null, "")]
    [TestCase("2023-10-3", null, "")]
    [TestCase(null, "2023-10-3", "")]
    [TestCase("2023-10-3 10:00:00", "2023-10-3 11:00:00", "3 October 2023, 10am to 11am")]
    [TestCase("2023-10-3 10:01:00", "2023-10-3 11:00:00", "3 October 2023, 10:01am to 11am")]
    [TestCase("2023-10-3 10:00:00", "2023-10-3 11:01:00", "3 October 2023, 10am to 11:01am")]
    [TestCase("2023-10-3 10:01:00", "2023-10-3 11:01:00", "3 October 2023, 10:01am to 11:01am")]
    [TestCase("2023-10-3 11:00:00", "2023-10-3 13:00:00", "3 October 2023, 11am to 1pm")]
    [TestCase("2023-10-3 11:30:00", "2023-10-3 13:00:00", "3 October 2023, 11:30am to 1pm")]
    [TestCase("2023-10-3 11:30:00", "2023-10-3 13:15:00", "3 October 2023, 11:30am to 1:15pm")]
    [TestCase("2024-11-4 12:00:00", "2024-11-4 13:30:00", "4 November 2024, 12pm to 1:30pm")]
    [TestCase("2025-12-5 12:01:00", "2025-12-5 13:00:00", "5 December 2025, 12:01pm to 1pm")]
    [TestCase("2025-12-5 12:01:00", "2025-12-5 17:30:00", "5 December 2025, 12:01pm to 5:30pm")]
    public void GetDateAndTimeFormatted_CheckResult(DateTime? start, DateTime? end, string expectedResult)
    {
        var vm = new CheckYourAnswersViewModel { Start = start, End = end };
        var result = vm.GetDateAndTimeFormatted();
        result.Should().Be(expectedResult);
    }
}