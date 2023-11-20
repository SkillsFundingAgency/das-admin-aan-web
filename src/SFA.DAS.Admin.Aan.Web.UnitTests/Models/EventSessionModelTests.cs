using AutoFixture.NUnit3;
using FluentAssertions;
using SFA.DAS.Aan.SharedUi.Constants;
using SFA.DAS.Aan.SharedUi.Models;
using SFA.DAS.Aan.SharedUi.OuterApi.Responses;
using SFA.DAS.Admin.Aan.Application.OuterApi.Calendar.Responses;
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
    public void SessionModel_ContainsExpectedStartValues(string? datetimeDescriptor, int? hour, int? minutes,
        bool isNullValue)
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
    public void SessionModel_ContainsExpectedEndValues(string? datetimeDescriptor, int? hour, int? minutes,
        bool isNullValue)
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
    public void Operator_CreateEventRequest_CheckLocationAgainstEventFormat(EventFormat eventFormat, string? location,
        string? expectedLocation)
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
    public void Operator_CreateEventRequest_CheckPostcodeAgainstEventFormat(EventFormat eventFormat, string? postcode,
        string? expected)
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
    public void Operator_CreateEventRequest_CheckLatitudeAgainstEventFormat(EventFormat eventFormat, double? latitude,
        double? expected)
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
    public void Operator_CreateEventRequest_CheckLongitudeAgainstEventFormat(EventFormat eventFormat, double? longitude,
        double? expected)
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
    public void Operator_CreateEventRequest_CheckEventLinkAgainstEventFormat(EventFormat eventFormat, string? link,
        string? expected)
    {
        var model = new EventSessionModel
        {
            EventFormat = eventFormat,
            EventLink = link
        };

        var request = (CreateEventRequest)model;

        request.EventLink.Should().Be(expected);
    }

    [TestCase("event summary 1")]
    [TestCase("event summary 2")]
    public void SessionModel_Summary_MapsToDescription(string eventSummary)
    {
        var model = new EventSessionModel { EventSummary = eventSummary };
        var request = (CreateEventRequest)model;
        request.Description.Should().Be(eventSummary);
    }

    [TestCase("event outline 1")]
    [TestCase("event outline 2")]
    public void SessionModel_Outline_MapsToSummary(string outline)
    {
        var model = new EventSessionModel { EventOutline = outline };
        var request = (CreateEventRequest)model;
        request.Summary.Should().Be(outline);
    }

    [Test, AutoData]
    public void Operator_MapsToNetworkEventDetailsViewModel(EventSessionModel source)
    {
        source.DateOfEvent = DateTime.Today.AddDays(1);
        source.StartHour = 12;
        source.StartMinutes = 40;
        source.EndHour = 14;
        source.EndMinutes = 10;
        var vm = (NetworkEventDetailsViewModel)source;
        vm.EventFormat.Should().Be(source.EventFormat);
        vm.LocationDetails.Should().BeEquivalentTo(new LocationDetails(source.Location, source.Postcode,
            source.Latitude, source.Longitude, null));
        vm.EventGuests.Count.Should().Be(source.GuestSpeakers.Count);
        vm.EventGuests.Should().BeEquivalentTo(source.GuestSpeakers
            .Select(guest => new EventGuest(guest.GuestName, guest.GuestJobTitle)).ToList());
        vm.IsPreview.Should().BeTrue();
    }

    [TestCase("Hybrid", EventFormat.Hybrid)]
    [TestCase("InPerson", EventFormat.InPerson)]
    [TestCase("Online", EventFormat.Online)]
    public void Operator_MappingGetCalendarEventQueryResult_EventFormat(string eventFormatString, EventFormat expected)
    {
        var source = new GetCalendarEventQueryResult
        {
            EventFormat = eventFormatString,
            EventGuests = new List<EventGuestModel>()
        };
        var result = (EventSessionModel)source;
        result.EventFormat.Should().Be(expected);
    }

    [Test]
    public void Operator_MappingGetCalendarEventQueryResult_GuestSpeakers()
    {
        var guestSpeakerName1 = "guest speaker 1";
        var guestSpeakerJobTitle1 = "guest speaker job 1";
        var expectedId1 = 1;
        var guestSpeakerName2 = "guest speaker 2";
        var guestSpeakerJobTitle2 = "guest speaker job 2";

        var eventGuests = new List<EventGuestModel>
        {
            new(guestSpeakerName1, guestSpeakerJobTitle1),
            new(guestSpeakerName2, guestSpeakerJobTitle2)
        };

        var source = new GetCalendarEventQueryResult
        {
            EventGuests = eventGuests
        };


        var result = (EventSessionModel)source;

        result.GuestSpeakers.FirstOrDefault().Should()
            .BeEquivalentTo(new GuestSpeaker(guestSpeakerName1, guestSpeakerJobTitle1, expectedId1));

        result.GuestSpeakers.Count.Should().Be(2);
    }

    [TestCase(null, "anything", EventSessionModel.NationalRegionName)]
    [TestCase(1, "East Midlands", "East Midlands")]
    public void Operator_MappingGetCalendarEventQueryResult_RegionName(int? regionId, string inputRegionName, string expectedRegionName)
    {

        var source = new GetCalendarEventQueryResult
        {
            EventGuests = new List<EventGuestModel>(),
            RegionId = regionId,
            RegionName = inputRegionName
        };

        var result = (EventSessionModel)source;
        result.RegionName.Should().Be(expectedRegionName);
        res.RegionName.Should().Be(expectedRegionName);
    }
}
