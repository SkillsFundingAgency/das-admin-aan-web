using FluentValidation.TestHelper;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;
using SFA.DAS.Admin.Aan.Web.Validators.CreateEvent;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Validators;
public class CreateEventDescriptionViewModelValidatorTests
{
    [TestCase(0, CreateEventDescriptionViewModelValidator.EventOutlineEmpty, false)]
    [TestCase(1, null, true)]
    [TestCase(CreateEventDescriptionViewModelValidator.EventOutlineMaxLength, null, true)]
    [TestCase(CreateEventDescriptionViewModelValidator.EventOutlineMaxLength + 1, CreateEventDescriptionViewModelValidator.EventOutlineTooLong, false)]
    public void Validate_EventOutline(int lengthOfOutline, string? errorMessage, bool isValid)
    {
        var model = new CreateEventDescriptionViewModel { EventOutline = new string('x', lengthOfOutline), EventSummary = "y", GuestSpeaker = true };

        var sut = new CreateEventDescriptionViewModelValidator();
        var result = sut.TestValidate(model);

        if (!isValid)
        {
            result.ShouldHaveValidationErrorFor(c => c.EventOutline)
                .WithErrorMessage(errorMessage);
        }
        else
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    [TestCase(0, CreateEventDescriptionViewModelValidator.EventSummaryEmpty, false)]
    [TestCase(1, null, true)]
    [TestCase(CreateEventDescriptionViewModelValidator.EventSummaryMaxLength, null, true)]
    [TestCase(CreateEventDescriptionViewModelValidator.EventSummaryMaxLength + 1, CreateEventDescriptionViewModelValidator.EventSummaryTooLong, false)]
    public void Validate_EventSummary(int lengthOfSummary, string? errorMessage, bool isValid)
    {
        var model = new CreateEventDescriptionViewModel { EventOutline = "x", EventSummary = new string('x', lengthOfSummary), GuestSpeaker = true };

        var sut = new CreateEventDescriptionViewModelValidator();
        var result = sut.TestValidate(model);

        if (!isValid)
        {
            result.ShouldHaveValidationErrorFor(c => c.EventSummary)
                .WithErrorMessage(errorMessage);
        }
        else
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(null, false)]
    public void Validate_GuestSpeaker_Check(bool? guestSpeaker, bool isValid)
    {
        var model = new CreateEventDescriptionViewModel { EventOutline = "x", EventSummary = "y", GuestSpeaker = guestSpeaker };

        var sut = new CreateEventDescriptionViewModelValidator();
        var result = sut.TestValidate(model);

        if (!isValid)
        {
            result.ShouldHaveValidationErrorFor(c => c.GuestSpeaker)
                .WithErrorMessage(CreateEventDescriptionViewModelValidator.GuestSpeakerEmpty);
        }
        else
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
