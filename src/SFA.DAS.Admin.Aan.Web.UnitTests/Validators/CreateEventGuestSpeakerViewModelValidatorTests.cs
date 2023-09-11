using FluentValidation.TestHelper;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;
using SFA.DAS.Admin.Aan.Web.Validators.CreateEvent;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Validators;

public class CreateEventGuestSpeakerViewModelValidatorTests
{

    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(null, false)]
    public void Validate_GuestSpeaker_Check(bool? guestSpeaker, bool isValid)
    {
        var model = new CreateEventHasGuestSpeakersViewModel { HasGuestSpeakers = guestSpeaker };

        var sut = new CreateEventGuestSpeakerViewModelValidator();
        var result = sut.TestValidate(model);

        if (!isValid)
        {
            result.ShouldHaveValidationErrorFor(c => c.HasGuestSpeakers)
                .WithErrorMessage(CreateEventGuestSpeakerViewModelValidator.GuestSpeakerEmpty);
        }
        else
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}