using FluentValidation;
using SFA.DAS.Admin.Aan.Application.Constants;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

namespace SFA.DAS.Admin.Aan.Web.Validators;

public class GuestSpeakerAddViewModelValidator : AbstractValidator<GuestSpeakerAddViewModel>
{
    public const int GuestSpeakerMaximumLength = 200;
    public const string NameEmpty = "You must include a guest speaker name";
    public const string NameHasExcludedCharacter = "Your guest name must be in alphanumeric text";
    public const string NameTooLong = "Your guest speakers name must be 200 characters or less";
    public const string JobRoleAndOrganisationEmpty = "You must include a guest speakers role and organisation";
    public const string JobRoleAndOrganisationHasExcludedCharacter = "Your guest speaker role and organisation must be in alphanumeric text";
    public const string JobRoleAndOrganisationTooLong = "Your guest speaker role and organisation must be 200 characters or less";

    public GuestSpeakerAddViewModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(NameEmpty)
            .Matches(RegularExpressions.ExcludedCharactersRegex)
            .WithMessage(NameHasExcludedCharacter)
            .MaximumLength(GuestSpeakerMaximumLength)
            .WithMessage(NameTooLong);

        RuleFor(x => x.JobRoleAndOrganisation)
            .NotEmpty()
            .WithMessage(JobRoleAndOrganisationEmpty)
            .Matches(RegularExpressions.ExcludedCharactersRegex)
            .WithMessage(JobRoleAndOrganisationHasExcludedCharacter)
            .MaximumLength(GuestSpeakerMaximumLength)
            .WithMessage(JobRoleAndOrganisationTooLong);

    }
}