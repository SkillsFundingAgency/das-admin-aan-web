using FluentValidation.TestHelper;
using SFA.DAS.Admin.Aan.Application.Constants;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;
using SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Validators.ManagerEvents;
public class OrganiserDetailsViewModelValidatorTests
{
    [TestCase(0, OrganiserDetailsViewModelValidator.OrganiserNameEmpty, false)]
    [TestCase(1, null, true)]
    [TestCase(ManageEventValidation.OrganiserNameMaximumLength, null, true)]
    [TestCase(ManageEventValidation.OrganiserNameMaximumLength + 1, OrganiserDetailsViewModelValidator.OrganiserNameTooLong, false)]
    public void Validate_OrganiserName(int lengthOfOrganiserName, string? errorMessage, bool isValid)
    {
        var model = new OrganiserDetailsViewModel
        { OrganiserName = new string('x', lengthOfOrganiserName), OrganiserEmail = "test@test.com" };

        var sut = new OrganiserDetailsViewModelValidator();
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

    [TestCase(0, OrganiserDetailsViewModelValidator.OrganiserEmailEmpty, false)]
    [TestCase(ManageEventValidation.OrganiserEmailMaximumLength, null, true)]
    [TestCase(ManageEventValidation.OrganiserEmailMaximumLength + 1, OrganiserDetailsViewModelValidator.OrganiserEmailTooLong, false)]
    public void Validate_OrganisationEmail_EmptyAndLength(int lengthOfOrganiserEmail, string? errorMessage, bool isValid)
    {
        var organiserEmail = string.Empty;

        if (lengthOfOrganiserEmail > 6)
        {
            organiserEmail = "a@" + new string('x', lengthOfOrganiserEmail - 6) + ".com";
        }

        var model = new OrganiserDetailsViewModel
        { OrganiserName = "test", OrganiserEmail = organiserEmail };

        var sut = new OrganiserDetailsViewModelValidator();
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
        var model = new OrganiserDetailsViewModel
        { OrganiserName = "test", OrganiserEmail = email };

        var sut = new OrganiserDetailsViewModelValidator();
        var result = sut.TestValidate(model);

        if (isValid)
            result.ShouldNotHaveValidationErrorFor(c => c.OrganiserEmail);
        else
            result.ShouldHaveValidationErrorFor(c => c.OrganiserEmail);
    }
}
