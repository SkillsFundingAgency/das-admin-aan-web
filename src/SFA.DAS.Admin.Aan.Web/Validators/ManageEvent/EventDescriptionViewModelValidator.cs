using FluentValidation;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

public class EventDescriptionViewModelValidator : AbstractValidator<EventDescriptionViewModel>
{

    public const string EventOutlineEmpty = "You must include an event outline";
    public const string EventOutlineTooLong = "Your event outline must be 200 characters or less";
    public const string EventSummaryEmpty = "You must include an event summary";
    public const string EventSummaryTooLong = "Your event summary must be 2000 characters or less";

    public EventDescriptionViewModelValidator()
    {
        RuleFor(x => x.EventOutline)
            .NotEmpty()
            .WithMessage(EventOutlineEmpty)
            .MaximumLength(Application.Constants.ManageEventValidation.EventOutlineMaxLength)
            .WithMessage(EventOutlineTooLong);

        RuleFor(x => x.EventSummary)
            .NotEmpty()
            .WithMessage(EventSummaryEmpty)
            .MaximumLength(Application.Constants.ManageEventValidation.EventSummaryMaxLength)
            .WithMessage(EventSummaryTooLong);
    }
}