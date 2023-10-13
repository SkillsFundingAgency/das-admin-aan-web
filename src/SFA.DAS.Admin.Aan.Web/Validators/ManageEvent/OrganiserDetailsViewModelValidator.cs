using FluentValidation;
using SFA.DAS.Admin.Aan.Application.Constants;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

public class OrganiserDetailsViewModelValidator : AbstractValidator<OrganiserDetailsViewModel>
{
    public const string OrganiserNameEmpty = "You must include an event organiser name";
    public const string OrganiserNameTooLong = "The name of your event organiser must be 200 characters or less";
    public const string OrganiserNameExcludedCharacter = "The name of your event organiser must not include any special characters: @, #, $, ^, =, +, \\, /, <, >, %";

    public const string OrganiserEmailEmpty = "You must include an event organiser email address";
    public const string OrganiserEmailTooLong = "The email address of your event organiser must be 256 characters or less";
    public const string OrganiserEmailWrongFormat = "Enter an email address in the correct format. For example, name@example.com";

    public OrganiserDetailsViewModelValidator()
    {
        RuleFor(x => x.OrganiserName)
            .NotEmpty()
            .WithMessage(OrganiserNameEmpty)
            .MaximumLength(ManageEventValidation.OrganiserNameMaximumLength)
            .WithMessage(OrganiserNameTooLong)
            .Matches(RegularExpressions.ExcludedCharactersRegex)
            .WithMessage(OrganiserNameExcludedCharacter);

        RuleFor(x => x.OrganiserEmail)
            .NotEmpty()
            .WithMessage(OrganiserEmailEmpty)
            .Matches(RegularExpressions.EmailRegex)
            .WithMessage(OrganiserEmailWrongFormat)
            .MaximumLength(ManageEventValidation.OrganiserEmailMaximumLength)
            .WithMessage(OrganiserEmailTooLong);
    }
}