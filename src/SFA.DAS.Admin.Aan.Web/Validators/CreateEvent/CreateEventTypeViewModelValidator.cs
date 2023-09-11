﻿using FluentValidation;
using SFA.DAS.Admin.Aan.Application.Constants;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

namespace SFA.DAS.Admin.Aan.Web.Validators.CreateEvent;

public class CreateEventTypeViewModelValidator : AbstractValidator<CreateEventTypeViewModel>
{
    public const string EventTitleEmpty = "You must include an event title";
    public const string EventTitleHasExcludedCharacter = "Your event title must not include any special characters: @, #, $, ^, =, +, \\, /, <, >, %";
    public const string EventTypeEmpty = "You must select an event type";
    public const string EventRegionEmpty = "You must select a region";

    public CreateEventTypeViewModelValidator()
    {
        RuleFor(x => x.EventTitle)
            .NotEmpty()
            .WithMessage(EventTitleEmpty)
            .Matches(RegularExpressions.ExcludedCharactersRegex)
            .WithMessage(EventTitleHasExcludedCharacter);

        RuleFor(x => x.EventTypeId)
            .NotEmpty()
            .WithMessage(EventTypeEmpty);

        RuleFor(x => x.EventRegionId)
            .NotEmpty()
            .WithMessage(EventRegionEmpty);
    }
}