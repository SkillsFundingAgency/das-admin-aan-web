using FluentValidation.TestHelper;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;
using SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Validators.ManagerEvents;

public class IsAtSchoolViewModelValidatorTests
{

    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(null, false)]
    public void Validate_EventAtSchool_Check(bool? isAtSchool, bool isValid)
    {
        var model = new IsAtSchoolViewModel { IsAtSchool = isAtSchool };

        var sut = new IsAtSchoolViewModelValidator();
        var result = sut.TestValidate(model);

        if (!isValid)
        {
            result.ShouldHaveValidationErrorFor(c => c.IsAtSchool)
                .WithErrorMessage(IsAtSchoolViewModelValidator.EventAtSchoolEmpty);
        }
        else
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}