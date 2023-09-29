using FluentValidation.TestHelper;
using SFA.DAS.Admin.Aan.Application.Constants;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;
using SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Validators;
public class EventSchoolNameViewModelValidatorTests
{
    [TestCase(0, null, EventSchoolNameViewModelValidator.EventSchoolNameEmpty, false)]
    [TestCase(1, null, null, true)]
    public void Validate_SchoolNameViaUrn(int lengthOfUrn, EventFormat? eventFormat, string? errorMessage, bool isValid)
    {
        var model = new EventSchoolNameViewModel
        { Urn = new string('x', lengthOfUrn) };

        var sut = new EventSchoolNameViewModelValidator();
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
