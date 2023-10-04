using FluentAssertions;
using SFA.DAS.Admin.Aan.Application.Constants;
using SFA.DAS.Admin.Aan.Application.OuterApi.CalendarEvents;
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

    [TestCase(true)]
    [TestCase(false)]
    public void Operator_CreateEventRequest_CheckGuestSpeakers(bool hasGuestSpeakers)
    {
        var guestSpeakers = new List<GuestSpeaker>
        {
            new GuestSpeaker("name1", "jobTitle1", 1),
            new GuestSpeaker("name2", "jobTitle2", 2)
        };

        var model = new EventSessionModel
        {
            HasGuestSpeakers = hasGuestSpeakers,
            GuestSpeakers = guestSpeakers
        };

        var request = (CreateEventRequest)model;

        if (hasGuestSpeakers)
        {
            request.Guests.Count.Should().Be(2);
        }
        else
        {
            request.Guests.Any().Should().BeFalse();
        }
    }
}
