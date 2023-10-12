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

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public void Validate_InPersonEventHasNoLocation_Error(string? eventLocation)
    {
        var model = GetHydratedModel();
        model.EventFormat = EventFormat.InPerson;
        model.EventLocation = eventLocation;

        var sut = new CheckYourAnswersViewModelValidator();
        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.EventLocation)
            .WithErrorMessage(CheckYourAnswersViewModelValidator.EventFormatHasLocationAndLocationEmpty);
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
            NumberOfAttendees = 42,
            EventFormatLink = ""
        };

        return vm;
    }
}