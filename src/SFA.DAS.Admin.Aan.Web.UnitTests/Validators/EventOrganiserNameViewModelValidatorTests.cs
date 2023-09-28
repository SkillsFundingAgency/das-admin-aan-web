using FluentValidation.TestHelper;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;
using SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Validators;
public class EventOrganiserNameViewModelValidatorTests
{
    [TestCase(0, EventOrganiserNameViewModelValidator.OrganiserNameEmpty, false)]
    [TestCase(1, null, true)]
    [TestCase(EventOrganiserNameViewModelValidator.OrganiserNameMaximumLength, null, true)]
    [TestCase(EventOrganiserNameViewModelValidator.OrganiserNameMaximumLength + 1, EventOrganiserNameViewModelValidator.OrganiserNameTooLong, false)]
    public void Validate_OrganiserName(int lengthOfOrganiserName, string? errorMessage, bool isValid)
    {
        var model = new EventOrganiserNameViewModel
        { OrganiserName = new string('x', lengthOfOrganiserName), OrganiserEmail = "test@test.com" };

        var sut = new EventOrganiserNameViewModelValidator();
        var result = sut.TestValidate(model);

        if (!isValid)
        {
            result.ShouldHaveValidationErrorFor(c => c.OrganiserName)
                .WithErrorMessage(errorMessage);
        }
        else
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
    }



    [TestCase(0, EventOrganiserNameViewModelValidator.OrganiserEmailEmpty, false)]
    [TestCase(EventOrganiserNameViewModelValidator.OrganiserEmailMaximumLength, null, true)]
    [TestCase(EventOrganiserNameViewModelValidator.OrganiserEmailMaximumLength + 1, EventOrganiserNameViewModelValidator.OrganiserEmailTooLong, false)]
    public void Validate_OrganisationEmail_EmptyAndLength(int lengthOfOrganiserEmail, string? errorMessage, bool isValid)
    {
        var organiserEmail = string.Empty;

        if (lengthOfOrganiserEmail > 6)
        {
            organiserEmail = "a@" + new string('x', lengthOfOrganiserEmail - 6) + ".com";
        }

        var model = new EventOrganiserNameViewModel
        { OrganiserName = "test", OrganiserEmail = organiserEmail };

        var sut = new EventOrganiserNameViewModelValidator();
        var result = sut.TestValidate(model);

        if (!isValid)
        {
            result.ShouldHaveValidationErrorFor(c => c.OrganiserEmail)
                .WithErrorMessage(errorMessage);
        }
        else
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    [TestCase("@example.com", false)]
    [TestCase("", false)]
    [TestCase(" ", false)]
    [TestCase("a@a", false)]
    [TestCase("email@email.com", true)]
    public void Validate_OrganiserEmail(string email, bool isValid)
    {
        var model = new EventOrganiserNameViewModel
        { OrganiserName = "test", OrganiserEmail = email };

        var sut = new EventOrganiserNameViewModelValidator();
        var result = sut.TestValidate(model);

        if (isValid)
            result.ShouldNotHaveValidationErrorFor(c => c.OrganiserEmail);
        else
            result.ShouldHaveValidationErrorFor(c => c.OrganiserEmail);
    }
}
