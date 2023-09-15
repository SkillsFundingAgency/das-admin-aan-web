using FluentValidation;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

namespace SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

public class HasGuestSpeakersViewModelValidator : AbstractValidator<HasGuestSpeakersViewModel>
{
    public const string GuestSpeakerEmpty = "You must select an option";

    public HasGuestSpeakersViewModelValidator()
    {
        RuleFor(x => x.HasGuestSpeakers)
            .NotEmpty()
            .WithMessage(GuestSpeakerEmpty);
    }
}