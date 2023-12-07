using FluentValidation.TestHelper;
using SFA.DAS.Aan.SharedUi.Constants;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;
using SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Validators.ManageEvents;

public class ReviewEventViewModelValidatorTests
{
    [Test]
    public void Validate_AllDetailsAreValid()
    {
        var model = GetHydratedModel();

        var sut = new ReviewEventViewModelValidator();
        var result = sut.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [TestCase(EventFormat.InPerson, null, false)]
    [TestCase(EventFormat.InPerson, "", false)]
    [TestCase(EventFormat.InPerson, " ", false)]
    [TestCase(EventFormat.Hybrid, null, false)]
    [TestCase(EventFormat.Hybrid, "", false)]
    [TestCase(EventFormat.Hybrid, " ", false)]
    [TestCase(EventFormat.Online, null, true)]
    [TestCase(EventFormat.Online, "", true)]
    [TestCase(EventFormat.Online, " ", true)]
    [TestCase(EventFormat.InPerson, "location", true)]
    [TestCase(EventFormat.Hybrid, "location", true)]
    [TestCase(EventFormat.Online, "location", true)]
    public void Validate_InPersonEventHasNoLocation_Error(EventFormat eventFormat, string? eventLocation, bool isValid)
    {
        var model = GetHydratedModel();
        model.EventFormat = eventFormat;
        model.EventLocation = eventLocation;

        var sut = new ReviewEventViewModelValidator();
        var result = sut.TestValidate(model);

        if (!isValid)
        {
            result.ShouldHaveValidationErrorFor(c => c.EventLocation)
                .WithErrorMessage(ReviewEventViewModelValidator.EventFormatHasLocationAndLocationEmpty);
        }
        else
        {
            result.ShouldNotHaveValidationErrorFor(c => c.EventLocation);
        }
    }

    [TestCase(true, "school name", true)]
    [TestCase(true, "", false)]
    [TestCase(true, null, false)]
    [TestCase(false, null, true)]
    [TestCase(false, "", true)]
    [TestCase(false, "school name", true)]
    public void Validate_InPersonEventHasNoLocation_Error(bool isAtSchool, string? schoolName, bool isValid)
    {
        var model = GetHydratedModel();
        model.IsAtSchool = isAtSchool;
        model.SchoolName = schoolName;

        var sut = new ReviewEventViewModelValidator();
        var result = sut.TestValidate(model);

        if (!isValid)
        {
            result.ShouldHaveValidationErrorFor(c => c.SchoolName)
                .WithErrorMessage(ReviewEventViewModelValidator.EventSchoolNameEmpty);
        }
        else
        {
            result.ShouldNotHaveValidationErrorFor(c => c.SchoolName);
        }
    }


    private static ReviewEventViewModel GetHydratedModel()
    {
        var vm = new ReviewEventViewModel
        {
            PreviewLink = "",
            HasSeenPreview = false,
            EventFormat = EventFormat.Hybrid,
            EventTitle = "title",
            EventType = "event type",
            EventRegion = "event region",
            EventOutline = "event outline",
            EventSummary = "event summary",
            HasGuestSpeakers = true,
            GuestSpeakers = new List<GuestSpeaker>
            {
                new GuestSpeaker("guest 1", "guest job title 1", 1),
                new GuestSpeaker("guest 2", "guest job title 2", 2)
            },
            Start = DateTime.Today.AddDays(1),
            End = DateTime.Today.AddDays(1).AddHours(1),
            EventLocation = "event location",
            OnlineEventLink = "https://www.test.com",
            SchoolName = "School name",
            IsAtSchool = true,
            OrganiserEmail = "organiser@test.com",
            OrganiserName = "organiser name",
            NumberOfAttendees = 42
        };

        return vm;
    }

    [TestCase(null, null, true)]
    [TestCase(null, EventFormat.Hybrid, true)]
    [TestCase(null, EventFormat.InPerson, true)]
    [TestCase(null, EventFormat.Online, true)]
    [TestCase("", null, true)]
    [TestCase("", EventFormat.Hybrid, true)]
    [TestCase("", EventFormat.InPerson, true)]
    [TestCase("", EventFormat.Online, true)]
    [TestCase("notAnUrl", null, true)]
    [TestCase("notAnUrl", EventFormat.Hybrid, false)]
    [TestCase("notAnUrl", EventFormat.InPerson, true)]
    [TestCase("notAnUrl", EventFormat.Online, false)]
    [TestCase("http://notsecureurl.com", EventFormat.Online, false)]
    [TestCase("http://notsecureurl.com", EventFormat.Hybrid, false)]
    [TestCase("https://secureurl.com", EventFormat.Online, true)]
    [TestCase("https://secureurl.com", EventFormat.Hybrid, true)]
    [TestCase("HTTPS://secureurl.com", EventFormat.Hybrid, true)]
    [TestCase("Https://secureurl.com", EventFormat.Hybrid, true)]
    [TestCase("hTtps://secureurl.com", EventFormat.Hybrid, true)]
    [TestCase("htTps://secureurl.com", EventFormat.Hybrid, true)]
    [TestCase("httPs://secureurl.com", EventFormat.Hybrid, true)]
    [TestCase("httpS://secureurl.com", EventFormat.Hybrid, true)]
    [TestCase("https://www.secureurl.com", EventFormat.Hybrid, true)]
    [TestCase("tts://w.something", EventFormat.Hybrid, false)]
    [TestCase("https://teams.microsoft.com", EventFormat.Online, true)]
    [TestCase("https://teams.microsoft.com/l/meetup-join/19%3ameeting_ZGRmZC00NzlkLWFmZTktOTU4ZmFkMjA2ZDE1%thread.v2/0?context=%7b%22ad277c9-c60a-4da1-b5f3-b3b8Oid%22%3a%2209a4-c6-48-bc-ad60%22%7d", EventFormat.Hybrid, true)]
    [TestCase("https://test-install.blindsidenetworks.com/bigbluebutton/api/join?meetingID=DemoMeeting", EventFormat.Online, true)]
    public void Validate_EventLink(string? url, EventFormat? eventFormat, bool isValid)
    {
        var model = new ReviewEventViewModel
        { EventLocation = "xyz", EventFormat = eventFormat, OnlineEventLink = url };

        var sut = new ReviewEventViewModelValidator();
        var result = sut.TestValidate(model);
        if (!isValid)
        {
            result.ShouldHaveValidationErrorFor(c => c.OnlineEventLink)
                .WithErrorMessage(ReviewEventViewModelValidator.EventLinkMustBeValid);
        }
        else
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}