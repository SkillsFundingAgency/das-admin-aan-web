using FluentValidation.TestHelper;
using SFA.DAS.Aan.SharedUi.Constants;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;
using SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Validators.ManageEvents;
public class LocationViewModelValidatorTests
{
    [TestCase(0, null, LocationViewModelValidator.EventLocationEmpty, true)]
    [TestCase(1, null, null, true)]
    [TestCase(0, EventFormat.Online, LocationViewModelValidator.EventLocationEmpty, true)]
    [TestCase(1, EventFormat.Online, null, true)]
    [TestCase(1, EventFormat.InPerson, null, true)]
    [TestCase(1, EventFormat.Hybrid, null, true)]
    [TestCase(0, EventFormat.InPerson, LocationViewModelValidator.EventLocationEmpty, false)]
    [TestCase(0, EventFormat.Hybrid, LocationViewModelValidator.EventLocationEmpty, false)]
    public void Validate_EventLocation(int lengthOfLocation, EventFormat? eventFormat, string? errorMessage, bool isValid)
    {
        var model = new LocationViewModel
        { Postcode = new string('x', lengthOfLocation), EventFormat = eventFormat };

        var sut = new LocationViewModelValidator();
        var result = sut.TestValidate(model);

        if (!isValid)
        {
            result.ShouldHaveValidationErrorFor(c => c.SearchTerm)
                .WithErrorMessage(errorMessage);
        }
        else
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
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
        var model = new LocationViewModel
        { Postcode = "xyz", EventFormat = eventFormat, OnlineEventLink = url };

        var sut = new LocationViewModelValidator();
        var result = sut.TestValidate(model);
        if (!isValid)
        {
            result.ShouldHaveValidationErrorFor(c => c.OnlineEventLink)
                .WithErrorMessage(LocationViewModelValidator.EventLinkMustBeValid);
        }
        else
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
