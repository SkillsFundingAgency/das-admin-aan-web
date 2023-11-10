using FluentValidation.TestHelper;
using SFA.DAS.Aan.SharedUi.Constants;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;
using SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Validators.ManageEvents;
public class SchoolNameViewModelValidatorTests
{
    [TestCase(0, null, SchoolNameViewModelValidator.EventSchoolNameEmpty, false)]
    [TestCase(1, null, null, true)]
    public void Validate_SchoolNameViaUrn(int lengthOfUrn, EventFormat? eventFormat, string? errorMessage, bool isValid)
    {
        var model = new SchoolNameViewModel
        { Urn = new string('x', lengthOfUrn) };

        var sut = new SchoolNameViewModelValidator();
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
}
