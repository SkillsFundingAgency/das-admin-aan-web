using FluentValidation.TestHelper;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;
using SFA.DAS.Admin.Aan.Web.Validators;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Validators.ManagerEvents;

public class EventAtSchoolViewModelValidatorTests
{

    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(null, false)]
    public void Validate_EventAtSchool_Check(bool? isAtSchool, bool isValid)
    {
        var model = new EventAtSchoolViewModel { IsAtSchool = isAtSchool };

        var sut = new EventAtSchoolViewModelValidator();
        var result = sut.TestValidate(model);

        if (!isValid)
        {
            result.ShouldHaveValidationErrorFor(c => c.IsAtSchool)
                .WithErrorMessage(EventAtSchoolViewModelValidator.EventAtSchoolEmpty);
        }
        else
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}