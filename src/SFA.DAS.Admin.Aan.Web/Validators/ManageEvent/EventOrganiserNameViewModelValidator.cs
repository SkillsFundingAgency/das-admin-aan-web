using FluentValidation;
using SFA.DAS.Admin.Aan.Application.Constants;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

namespace SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

public class EventOrganiserNameViewModelValidator : AbstractValidator<EventOrganiserNameViewModel>
{
    public const int OrganiserNameMaximumLength = 200;
    public const string OrganiserNameEmpty = "You must include an event organiser name";
    public const string OrganiserNameTooLong = "The name of your event organiser must be 200 characters or less";

    public const int OrganiserEmailMaximumLength = 256;
    public const string OrganiserEmailEmpty = "You must include an event organiser email address";
    public const string OrganiserEmailTooLong = "The email address of your event organiser must be 256 characters or less";
    public const string OrganiserEmailWrongFormat = "Enter an email address in the correct format. For example, name@example.com";

    public EventOrganiserNameViewModelValidator()
    {
        RuleFor(x => x.OrganiserName)
            .NotEmpty()
            .WithMessage(OrganiserNameEmpty)
            .MaximumLength(OrganiserNameMaximumLength)
            .WithMessage(OrganiserNameTooLong);

        RuleFor(x => x.OrganiserEmail)
            .NotEmpty()
            .WithMessage(OrganiserEmailEmpty)
            .Matches(RegularExpressions.EmailRegex)
            .WithMessage(OrganiserEmailWrongFormat)
            .MaximumLength(OrganiserEmailMaximumLength)
            .WithMessage(OrganiserEmailTooLong);
    }
}