using FluentValidation.TestHelper;
using SFA.DAS.Admin.Aan.Application.Constants;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;
using SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Validators.ManageEvents;

public class CheckYourAnswersViewModelValidatorTests

{
    [Test]
    public void Validate_AllDetailsAreValid()
    {
        var model = GetHydratedModel();

        var sut = new CheckYourAnswersViewModelValidator();
        var result = sut.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    // [TestCase(EventFormat.InPerson, null, false)]
    // [TestCase(EventFormat.InPerson, "", false)]
    // [TestCase(EventFormat.InPerson, " ", false)]
    // [TestCase(EventFormat.Hybrid, null, false)]
    // [TestCase(EventFormat.Hybrid, "", false)]
    // [TestCase(EventFormat.Hybrid, " ", false)]
    // [TestCase(EventFormat.Online, null, true)]
    // [TestCase(EventFormat.Online, "", true)]
    // [TestCase(EventFormat.Online, " ", true)]
    // [TestCase(EventFormat.InPerson, "location", true)]
    // [TestCase(EventFormat.Hybrid, "location", true)]
    // [TestCase(EventFormat.Online, "location", true)]
    // public void Validate_InPersonEventHasNoLocation_Error(EventFormat eventFormat, string? eventLocation, bool isValid)
    // {
    //     var model = GetHydratedModel();
    //     model.EventFormat = eventFormat;
    //     model.EventLocation = eventLocation;
    //
    //     var sut = new CheckYourAnswersViewModelValidator();
    //     var result = sut.TestValidate(model);
    //
    //     if (!isValid)
    //     {
    //         result.ShouldHaveValidationErrorFor(c => c.EventLocation)
    //             .WithErrorMessage(CheckYourAnswersViewModelValidator.EventFormatHasLocationAndLocationEmpty);
    //     }
    //     else
    //     {
    //         result.ShouldNotHaveValidationErrorFor(c => c.EventLocation);
    //     }
    // }

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

        var sut = new CheckYourAnswersViewModelValidator();
        var result = sut.TestValidate(model);

        if (!isValid)
        {
            result.ShouldHaveValidationErrorFor(c => c.SchoolName)
                .WithErrorMessage(CheckYourAnswersViewModelValidator.EventSchoolNameEmpty);
        }
        else
        {
            result.ShouldNotHaveValidationErrorFor(c => c.SchoolName);
        }
    }


    private static CheckYourAnswersViewModel GetHydratedModel()
    {
        var vm = new CheckYourAnswersViewModel
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
}