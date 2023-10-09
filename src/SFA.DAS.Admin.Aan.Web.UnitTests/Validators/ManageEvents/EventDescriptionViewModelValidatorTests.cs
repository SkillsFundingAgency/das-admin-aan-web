using FluentValidation.TestHelper;
using SFA.DAS.Admin.Aan.Application.Constants;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;
using SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Validators.ManageEvents;

public class EventDescriptionViewModelValidatorTests
{
    [TestCase(0, EventDescriptionViewModelValidator.EventOutlineEmpty, false)]
    [TestCase(1, null, true)]
    [TestCase(ManageEventValidation.EventOutlineMaxLength, null, true)]
    [TestCase(ManageEventValidation.EventOutlineMaxLength + 1,
        EventDescriptionViewModelValidator.EventOutlineTooLong, false)]
    public void Validate_EventOutline(int lengthOfOutline, string? errorMessage, bool isValid)
    {
        var model = new EventDescriptionViewModel
        { EventOutline = new string('x', lengthOfOutline), EventSummary = "y" };

        var sut = new EventDescriptionViewModelValidator();
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

    [TestCase(0, EventDescriptionViewModelValidator.EventSummaryEmpty, false)]
    [TestCase(1, null, true)]
    [TestCase(ManageEventValidation.EventSummaryMaxLength, null, true)]
    [TestCase(ManageEventValidation.EventSummaryMaxLength + 1,
        EventDescriptionViewModelValidator.EventSummaryTooLong, false)]
    public void Validate_EventSummary(int lengthOfSummary, string? errorMessage, bool isValid)
    {
        var model = new EventDescriptionViewModel
        { EventOutline = "x", EventSummary = new string('x', lengthOfSummary) };

        var sut = new EventDescriptionViewModelValidator();
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