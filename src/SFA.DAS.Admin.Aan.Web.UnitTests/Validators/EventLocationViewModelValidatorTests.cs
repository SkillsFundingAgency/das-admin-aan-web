using FluentValidation.TestHelper;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;
using SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Validators;
public class EventLocationViewModelValidatorTests
{
    [TestCase(0, EventLocationViewModelValidator.EventLocationEmpty, false)]
    [TestCase(1, null, true)]
    public void Validate_EventLocation(int lengthOfLocation, string? errorMessage, bool isValid)
    {
        var model = new EventLocationViewModel
        { Postcode = new string('x', lengthOfLocation) };

        var sut = new EventLocationViewModelValidator();
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
