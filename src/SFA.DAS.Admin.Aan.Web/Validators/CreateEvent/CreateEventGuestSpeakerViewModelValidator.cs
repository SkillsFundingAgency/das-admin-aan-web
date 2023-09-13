using FluentValidation;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

namespace SFA.DAS.Admin.Aan.Web.Validators.CreateEvent;

public class CreateEventGuestSpeakerViewModelValidator : AbstractValidator<CreateEventHasGuestSpeakersViewModel>
{
    public const string GuestSpeakerEmpty = "You must select an option";

    public CreateEventGuestSpeakerViewModelValidator()
    {
        RuleFor(x => x.HasGuestSpeakers)
            .NotEmpty()
            .WithMessage(GuestSpeakerEmpty);
    }
}