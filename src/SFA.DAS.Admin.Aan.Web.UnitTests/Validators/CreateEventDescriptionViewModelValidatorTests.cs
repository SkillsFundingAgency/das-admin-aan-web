using FluentValidation.TestHelper;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;
using SFA.DAS.Admin.Aan.Web.Validators.CreateEvent;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Validators;

public class CreateEventDescriptionViewModelValidatorTests
{
    [TestCase(0, CreateEventDescriptionViewModelValidator.EventOutlineEmpty, false)]
    [TestCase(1, null, true)]
    [TestCase(CreateEventDescriptionViewModelValidator.EventOutlineMaxLength, null, true)]
    [TestCase(CreateEventDescriptionViewModelValidator.EventOutlineMaxLength + 1,
        CreateEventDescriptionViewModelValidator.EventOutlineTooLong, false)]
    public void Validate_EventOutline(int lengthOfOutline, string? errorMessage, bool isValid)
    {
        var model = new CreateEventDescriptionViewModel
        { EventOutline = new string('x', lengthOfOutline), EventSummary = "y" };

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
    [TestCase(CreateEventDescriptionViewModelValidator.EventSummaryMaxLength + 1,
        CreateEventDescriptionViewModelValidator.EventSummaryTooLong, false)]
    public void Validate_EventSummary(int lengthOfSummary, string? errorMessage, bool isValid)
    {
        var model = new CreateEventDescriptionViewModel
        { EventOutline = "x", EventSummary = new string('x', lengthOfSummary) };

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
}