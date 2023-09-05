using FluentValidation;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

namespace SFA.DAS.Admin.Aan.Web.Validators.CreateEvent;

public class CreateEventDescriptionViewModelValidator : AbstractValidator<CreateEventDescriptionViewModel>
{
    public const int EventOutlineMaxLength = 200;
    public const int EventSummaryMaxLength = 2000;
    public const string EventOutlineEmpty = "You must include an event outline";
    public const string EventOutlineTooLong = "Your event outline must be 200 characters or less";
    public const string EventSummaryEmpty = "You must include an event summary";
    public const string EventSummaryTooLong = "Your event summary must be 2000 characters or less";
    public const string GuestSpeakerEmpty = "You must select an option";

    public CreateEventDescriptionViewModelValidator()
    {
        RuleFor(x => x.EventOutline)
            .NotEmpty()
            .WithMessage(EventOutlineEmpty)
            .MaximumLength(EventOutlineMaxLength)
            .WithMessage(EventOutlineTooLong);

        RuleFor(x => x.EventSummary)
            .NotEmpty()
            .WithMessage(EventSummaryEmpty)
            .MaximumLength(EventSummaryMaxLength)
            .WithMessage(EventSummaryTooLong);

        RuleFor(x => x.GuestSpeaker)
            .NotEmpty()
            .WithMessage(GuestSpeakerEmpty);
    }
}