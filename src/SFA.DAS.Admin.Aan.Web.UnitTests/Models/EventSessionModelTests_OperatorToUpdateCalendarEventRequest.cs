using FluentAssertions;
using SFA.DAS.Aan.SharedUi.Constants;
using SFA.DAS.Admin.Aan.Application.OuterApi.CalendarEvents;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Models;
public class EventSessionModelTestsOperatorToUpdateCalendarEventRequest
{
    [TestCase(true)]
    [TestCase(false)]
    public void Operator_UpdateEventRequest_CheckGuestSpeakers(bool hasGuestSpeakers)
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

        var request = (UpdateCalendarEventRequest)model;

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
    public void Operator_UpdateEventRequest_CheckUrn(string? urn, bool? isAtSchool, long? expectedUrn)
    {
        var model = new EventSessionModel
        {
            Urn = urn,
            IsAtSchool = isAtSchool
        };

        var request = (UpdateCalendarEventRequest)model;

        request.Urn.Should().Be(expectedUrn);
    }

    [TestCase(null, true)]
    [TestCase("2050-10-10 10:30:00", false)]
    public void Operator_UpdateEventRequest_CheckStartDate(DateTime? startDate, bool startDateIsNull)
    {
        var model = new EventSessionModel
        {
            Start = startDate
        };

        var request = (UpdateCalendarEventRequest)model;
        if (startDateIsNull)
        {
            request.StartDate.Should().BeNull();
        }
        else
        {
            request.StartDate.Should().Be(startDate!.Value);
        }
    }

    [TestCase(null, true)]
    [TestCase("2050-10-10 10:30:00", false)]
    public void Operator_UpdateEventRequest_CheckEndDate(DateTime? endDate, bool endDateIsNull)
    {
        var model = new EventSessionModel
        {
            End = endDate
        };

        var request = (UpdateCalendarEventRequest)model;
        if (endDateIsNull)
        {
            request.EndDate.Should().BeNull();
        }
        else
        {
            request.EndDate.Should().Be(endDate!.Value);
        }
    }

    [TestCase(EventFormat.InPerson, "location", "location")]
    [TestCase(EventFormat.Hybrid, "location", "location")]
    [TestCase(EventFormat.Online, "location", null)]
    public void Operator_UpdateEventRequest_CheckLocationAgainstEventFormat(EventFormat eventFormat, string? location,
        string? expectedLocation)
    {
        var model = new EventSessionModel
        {
            EventFormat = eventFormat,
            Location = location
        };

        var request = (UpdateCalendarEventRequest)model;

        request.Location.Should().Be(expectedLocation);
    }

    [TestCase(EventFormat.InPerson, "location", "location")]
    [TestCase(EventFormat.Hybrid, "location", "location")]
    [TestCase(EventFormat.Online, "location", null)]
    public void Operator_UpdateEventRequest_CheckPostcodeAgainstEventFormat(EventFormat eventFormat, string? postcode,
        string? expected)
    {
        var model = new EventSessionModel
        {
            EventFormat = eventFormat,
            Postcode = postcode
        };

        var request = (UpdateCalendarEventRequest)model;

        request.Postcode.Should().Be(expected);
    }

    [TestCase(EventFormat.InPerson, 12, 12)]
    [TestCase(EventFormat.Hybrid, 13, 13)]
    [TestCase(EventFormat.Online, 14, null)]
    public void Operator_UpdateEventRequest_CheckLatitudeAgainstEventFormat(EventFormat eventFormat, double? latitude,
        double? expected)
    {
        var model = new EventSessionModel
        {
            EventFormat = eventFormat,
            Latitude = latitude
        };

        var request = (UpdateCalendarEventRequest)model;

        request.Latitude.Should().Be(expected);
    }

    [TestCase(EventFormat.InPerson, 12, 12)]
    [TestCase(EventFormat.Hybrid, 13, 13)]
    [TestCase(EventFormat.Online, 14, null)]
    public void Operator_UpdateEventRequest_CheckLongitudeAgainstEventFormat(EventFormat eventFormat, double? longitude,
        double? expected)
    {
        var model = new EventSessionModel
        {
            EventFormat = eventFormat,
            Longitude = longitude
        };

        var request = (UpdateCalendarEventRequest)model;

        request.Longitude.Should().Be(expected);
    }

    [TestCase(EventFormat.InPerson, "link", null)]
    [TestCase(EventFormat.Hybrid, "link", "link")]
    [TestCase(EventFormat.Online, "link", "link")]
    public void Operator_UpdateEventRequest_CheckEventLinkAgainstEventFormat(EventFormat eventFormat, string? link,
        string? expected)
    {
        var model = new EventSessionModel
        {
            EventFormat = eventFormat,
            EventLink = link
        };

        var request = (UpdateCalendarEventRequest)model;

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
        var request = (UpdateCalendarEventRequest)model;
        request.Summary.Should().Be(outline);
    }
}
