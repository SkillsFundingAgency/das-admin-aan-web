using FluentValidation.TestHelper;
using SFA.DAS.Admin.Aan.Web.Models.DeleteEvent;
using SFA.DAS.Admin.Aan.Web.Validators;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Validators;
public class DeleteEventViewModelValidatorTests
{

    [TestCase(true, true)]
    [TestCase(false, false)]
    public void Validate_GuestSpeaker_Check(bool isCancelConfirmed, bool isValid)
    {
        var model = new DeleteEventViewModel { IsCancelConfirmed = isCancelConfirmed };

        var sut = new DeleteEventViewModelValidator();
        var result = sut.TestValidate(model);

        if (!isValid)
        {
            result.ShouldHaveValidationErrorFor(c => c.IsCancelConfirmed)
                .WithErrorMessage(DeleteEventViewModelValidator.ConfirmCancelEventNotPicked);
        }
        else
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
