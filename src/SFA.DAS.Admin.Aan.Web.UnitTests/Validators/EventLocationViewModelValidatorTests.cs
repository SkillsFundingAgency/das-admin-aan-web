using FluentValidation.TestHelper;
using SFA.DAS.Admin.Aan.Application.Constants;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;
using SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Validators;
public class EventLocationViewModelValidatorTests
{
    [TestCase(0, null, EventLocationViewModelValidator.EventLocationEmpty, true)]
    [TestCase(1, null, null, true)]
    [TestCase(0, EventFormat.Online, EventLocationViewModelValidator.EventLocationEmpty, true)]
    [TestCase(1, EventFormat.Online, null, true)]
    [TestCase(1, EventFormat.InPerson, null, true)]
    [TestCase(1, EventFormat.Hybrid, null, true)]
    [TestCase(0, EventFormat.InPerson, EventLocationViewModelValidator.EventLocationEmpty, false)]
    [TestCase(0, EventFormat.Hybrid, EventLocationViewModelValidator.EventLocationEmpty, false)]
    public void Validate_EventLocation(int lengthOfLocation, EventFormat? eventFormat, string? errorMessage, bool isValid)
    {
        var model = new EventLocationViewModel
        { Postcode = new string('x', lengthOfLocation), EventFormat = eventFormat };

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
