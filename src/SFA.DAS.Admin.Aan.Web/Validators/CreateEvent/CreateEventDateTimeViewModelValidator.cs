using FluentValidation;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

namespace SFA.DAS.Admin.Aan.Web.Validators.CreateEvent;

public class CreateEventDateTimeViewModelValidator : AbstractValidator<CreateEventDateTimeViewModel>
{
    // public const string EventTitleEmpty = "You must include an event title";
    // public const string EventTitleHasExcludedCharacter = "Your event title must not include any special characters: @, #, $, ^, =, +, \\, /, <, >, %";
    // public const string EventTypeEmpty = "You must select an event type";
    // public const string EventRegionEmpty = "You must select a region";

    public CreateEventDateTimeViewModelValidator()
    {
        // RuleFor(x => x.EventTitle)
        //     .NotEmpty()
        //     .WithMessage(EventTitleEmpty)
        //     .Matches(RegularExpressions.ExcludedCharactersRegex)
        //     .WithMessage(EventTitleHasExcludedCharacter);
        //
        // RuleFor(x => x.EventTypeId)
        //     .NotEmpty()
        //     .WithMessage(EventTypeEmpty);
        //
        // RuleFor(x => x.EventRegionId)
        //     .NotEmpty()
        //     .WithMessage(EventRegionEmpty);
    }
}