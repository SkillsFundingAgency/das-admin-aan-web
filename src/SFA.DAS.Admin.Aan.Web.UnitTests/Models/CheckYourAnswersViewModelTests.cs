namespace SFA.DAS.Admin.Aan.Web.UnitTests.Models;

public class CheckYourAnswersViewModelTests
{
    // [Test, AutoData]
    // public void Operator_GivenEventSessionModel_ReturnsViewModel(EventSessionModel source)
    // {
    //     source.StartHour = 12;
    //     source.StartMinutes = 25;
    //     source.EndHour = 13;
    //     source.EndMinutes = 30;
    //     source.IsAlreadyPublished = false;
    //     ReviewEventViewModel sut = source;
    //     sut.Should().BeEquivalentTo(source, options => options.ExcludingMissingMembers());
    // }
    //
    // [TestCase("location 1", "AB1 CTX", "location 1, AB1 CTX")]
    // [TestCase("location 1", "", "location 1")]
    // [TestCase("location 1", null, "location 1")]
    // public void Operator_GetFullLocation_CheckResult(string location, string? postcode, string expected)
    // {
    //     var source = new EventSessionModel
    //     {
    //         Location = location,
    //         Postcode = postcode,
    //         StartHour = 12,
    //         StartMinutes = 25,
    //         EndHour = 13,
    //         EndMinutes = 30
    //     };
    //
    //     var vm = (ReviewEventViewModel)source;
    //     var result = vm.EventLocation;
    //     result.Should().Be(expected);
    // }
    //
    // [TestCase(EventFormat.InPerson, true)]
    // [TestCase(EventFormat.Hybrid, true)]
    // [TestCase(EventFormat.Online, false)]
    // public void ViewModel_ShowEventFormatCheck(EventFormat eventFormat, bool expectedShowLocation)
    // {
    //     var vm = new ReviewEventViewModel { EventFormat = eventFormat };
    //     vm.ShowLocation.Should().Be(expectedShowLocation);
    // }
    //
    // [TestCase(EventFormat.InPerson, false)]
    // [TestCase(EventFormat.Hybrid, true)]
    // [TestCase(EventFormat.Online, true)]
    // public void ViewModel_ShowEventLinkCheck(EventFormat eventFormat, bool expectedShowEventLink)
    // {
    //     var vm = new ReviewEventViewModel { EventFormat = eventFormat };
    //     vm.ShowOnlineEventLink.Should().Be(expectedShowEventLink);
    // }
    //
    // [TestCase(null, null, "")]
    // [TestCase("2023-10-3", null, "")]
    // [TestCase(null, "2023-10-3", "")]
    // [TestCase("2023-10-3 10:00:00", "2023-10-3 11:00:00", "3 October 2023, 10:00am to 11:00am")]
    // [TestCase("2023-10-3 10:01:00", "2023-10-3 11:00:00", "3 October 2023, 10:01am to 11:00am")]
    // [TestCase("2023-10-3 10:00:00", "2023-10-3 11:01:00", "3 October 2023, 10:00am to 11:01am")]
    // [TestCase("2023-10-3 10:01:00", "2023-10-3 11:01:00", "3 October 2023, 10:01am to 11:01am")]
    // [TestCase("2023-10-3 11:00:00", "2023-10-3 13:00:00", "3 October 2023, 11:00am to 1:00pm")]
    // [TestCase("2023-10-3 11:30:00", "2023-10-3 13:00:00", "3 October 2023, 11:30am to 1:00pm")]
    // [TestCase("2023-10-3 11:30:00", "2023-10-3 13:15:00", "3 October 2023, 11:30am to 1:15pm")]
    // [TestCase("2024-11-4 12:00:00", "2024-11-4 13:30:00", "4 November 2024, 12:00pm to 1:30pm")]
    // [TestCase("2025-12-5 12:01:00", "2025-12-5 13:00:00", "5 December 2025, 12:01pm to 1:00pm")]
    // [TestCase("2025-12-5 12:01:00", "2025-12-5 17:30:00", "5 December 2025, 12:01pm to 5:30pm")]
    // public void GetDateAndTimeFormatted_CheckResult(DateTime? start, DateTime? end, string expectedResult)
    // {
    //     var vm = new ReviewEventViewModel { Start = start, End = end };
    //     var result = vm.GetDateAndTimeFormatted();
    //     result.Should().Be(expectedResult);
    // }
    //
    // [TestCase(false, CreateEvent.PageTitle)]
    // [TestCase(true, UpdateEvent.PageTitle)]
    // public void SessionModel_ContainsExpectedPageTitle(bool isAlreadyPublished, string pageTitle)
    // {
    //     var vm = new EventSessionModel { IsAlreadyPublished = isAlreadyPublished };
    //     vm.PageTitle.Should().Be(pageTitle);
    // }
}