using FluentValidation.TestHelper;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;
using SFA.DAS.Admin.Aan.Web.Validators;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Validators.ManagerEvents;

public class HasGuestSpeakersViewModelValidatorTests
{

    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(null, false)]
    public void Validate_GuestSpeaker_Check(bool? guestSpeaker, bool isValid)
    {
        var model = new HasGuestSpeakersViewModel { HasGuestSpeakers = guestSpeaker };

        var sut = new HasGuestSpeakersViewModelValidator();
        var result = sut.TestValidate(model);

        if (!isValid)
        {
            result.ShouldHaveValidationErrorFor(c => c.HasGuestSpeakers)
                .WithErrorMessage(HasGuestSpeakersViewModelValidator.GuestSpeakerEmpty);
        }
        else
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}