using FluentAssertions;
using SFA.DAS.Admin.Aan.Application.Constants;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Models;
public class EventSessionModelTests
{
    [TestCase(EventFormat.InPerson)]
    [TestCase(EventFormat.Hybrid)]
    [TestCase(EventFormat.Online)]
    [TestCase(null)]
    public void SessionModel_ContainsExpectedValues(EventFormat? eventFormat)
    {
        var vm = new EventSessionModel { EventFormat = eventFormat };
        vm.EventFormat.Should().Be(eventFormat);
        vm.HasSeenPreview.Should().Be(false);
    }


    [TestCase("2035-12-01", 12, 1, false)]
    [TestCase("2035-12-01", 12, 0, false)]
    [TestCase(null, 12, 0, true)]
    [TestCase("2035-12-01", null, 0, true)]
    [TestCase("2035-12-01", 12, null, true)]
    public void SessionModel_ContainsExpectedStartValues(string? datetimeDescriptor, int? hour, int? minutes, bool isNullValue)
    {
        var dateTime = (DateTime?)null;

        if (!string.IsNullOrEmpty(datetimeDescriptor)) dateTime = DateTime.Parse(datetimeDescriptor);

        var vm = new EventSessionModel { DateOfEvent = dateTime, StartHour = hour, StartMinutes = minutes };

        if (isNullValue)
            vm.Start.Should().BeNull();
        else
        {
            var expectedDate = new DateTime(dateTime!.Value.Year, dateTime.Value.Month, dateTime.Value.Day, hour!.Value,
                minutes!.Value, 0);
            vm.Start.Should().Be(expectedDate);
        }
    }

    [TestCase("2035-12-01", 12, 1, false)]
    [TestCase("2035-12-01", 12, 0, false)]
    [TestCase(null, 12, 0, true)]
    [TestCase("2035-12-01", null, 0, true)]
    [TestCase("2035-12-01", 12, null, true)]
    public void SessionModel_ContainsExpectedEndValues(string? datetimeDescriptor, int? hour, int? minutes, bool isNullValue)
    {
        var dateTime = (DateTime?)null;

        if (!string.IsNullOrEmpty(datetimeDescriptor)) dateTime = DateTime.Parse(datetimeDescriptor);

        var vm = new EventSessionModel { DateOfEvent = dateTime, EndHour = hour, EndMinutes = minutes };

        if (isNullValue)
            vm.End.Should().BeNull();
        else
        {
            var expectedDate = new DateTime(dateTime!.Value.Year, dateTime.Value.Month, dateTime.Value.Day, hour!.Value,
                minutes!.Value, 0);
            vm.End.Should().Be(expectedDate);
        }
    }
}
