using FluentValidation;
using SFA.DAS.Admin.Aan.Application.Constants;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

public class EventDescriptionViewModelValidator : AbstractValidator<EventDescriptionViewModel>
{

    public const string EventOutlineEmpty = "You must include an event outline";
    public const string EventOutlineTooLong = "Your event outline must be 200 characters or less";
    public const string EventOutlineHasExcludedCharacter = "Your event outline must not include any special characters: @, #, $, ^, =, +, \\, /, <, >, %";

    public const string EventSummaryEmpty = "You must include an event summary";
    public const string EventSummaryTooLong = "Your event summary must be 2000 characters or less";
    public const string EventSummaryHasExcludedCharacter = "Your event summary must not include any special characters: <, >";

    public EventDescriptionViewModelValidator()
    {
        RuleFor(x => x.EventOutline)
            .NotEmpty()
            .WithMessage(EventOutlineEmpty)
            .MaximumLength(ManageEventValidation.EventOutlineMaxLength)
            .WithMessage(EventOutlineTooLong)
            .Matches(RegularExpressions.ExcludedCharactersRegex)
            .WithMessage(EventOutlineHasExcludedCharacter);

        RuleFor(x => x.EventSummary)
            .NotEmpty()
            .WithMessage(EventSummaryEmpty)
            .MaximumLength(ManageEventValidation.EventSummaryMaxLength)
            .WithMessage(EventSummaryTooLong)
            .Matches(RegularExpressions.ExcludedCharactersInMarkdownRegex)
            .WithMessage(EventSummaryHasExcludedCharacter);
    }
}