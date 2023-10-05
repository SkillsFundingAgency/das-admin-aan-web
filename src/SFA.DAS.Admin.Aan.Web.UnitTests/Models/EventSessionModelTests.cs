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

    [TestCase(null, null, null)]
    [TestCase(null, true, null)]
    [TestCase(null, false, null)]
    [TestCase("abc", true, null)]
    [TestCase("123", false, null)]
    [TestCase("123", true, 123)]
    public void Operator_CreateEventRequest_CheckUrn(string? urn, bool? isAtSchool, long? expectedUrn)
    {
        var model = new EventSessionModel
        {
            Urn = urn,
            IsAtSchool = isAtSchool
        };

        var request = (CreateEventRequest)model;

        request.Urn.Should().Be(expectedUrn);
    }

    [TestCase(EventFormat.InPerson, "location", "location")]
    [TestCase(EventFormat.Hybrid, "location", "location")]
    [TestCase(EventFormat.Online, "location", null)]
    public void Operator_CreateEventRequest_CheckLocationAgainstEventFormat(EventFormat eventFormat, string? location, string? expectedLocation)
    {
        var model = new EventSessionModel
        {
            EventFormat = eventFormat,
            Location = location
        };

        var request = (CreateEventRequest)model;

        request.Location.Should().Be(expectedLocation);
    }

    [TestCase(EventFormat.InPerson, "location", "location")]
    [TestCase(EventFormat.Hybrid, "location", "location")]
    [TestCase(EventFormat.Online, "location", null)]
    public void Operator_CreateEventRequest_CheckPostcodeAgainstEventFormat(EventFormat eventFormat, string? postcode, string? expected)
    {
        var model = new EventSessionModel
        {
            EventFormat = eventFormat,
            Postcode = postcode
        };

        var request = (CreateEventRequest)model;

        request.Postcode.Should().Be(expected);
    }

    [TestCase(EventFormat.InPerson, 12, 12)]
    [TestCase(EventFormat.Hybrid, 13, 13)]
    [TestCase(EventFormat.Online, 14, null)]
    public void Operator_CreateEventRequest_CheckLatitudeAgainstEventFormat(EventFormat eventFormat, double? latitude, double? expected)
    {
        var model = new EventSessionModel
        {
            EventFormat = eventFormat,
            Latitude = latitude
        };

        var request = (CreateEventRequest)model;

        request.Latitude.Should().Be(expected);
    }

    [TestCase(EventFormat.InPerson, 12, 12)]
    [TestCase(EventFormat.Hybrid, 13, 13)]
    [TestCase(EventFormat.Online, 14, null)]
    public void Operator_CreateEventRequest_CheckLongitudeAgainstEventFormat(EventFormat eventFormat, double? longitude, double? expected)
    {
        var model = new EventSessionModel
        {
            EventFormat = eventFormat,
            Longitude = longitude
        };

        var request = (CreateEventRequest)model;

        request.Longitude.Should().Be(expected);
    }

    [TestCase(EventFormat.InPerson, "link", null)]
    [TestCase(EventFormat.Hybrid, "link", "link")]
    [TestCase(EventFormat.Online, "link", "link")]
    public void Operator_CreateEventRequest_CheckEventLinkAgainstEventFormat(EventFormat eventFormat, string? link, string? expected)
    {
        var model = new EventSessionModel
        {
            EventFormat = eventFormat,
            EventLink = link
        };

        var request = (CreateEventRequest)model;

        request.EventLink.Should().Be(expected);
    }
}
