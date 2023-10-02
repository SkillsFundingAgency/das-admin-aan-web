using FluentValidation;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

namespace SFA.DAS.Admin.Aan.Web.Validators;

public class EventFormatViewModelValidator : AbstractValidator<EventFormatViewModel>
{
    public const string EventFormatErrorMessage = "You must select an event format";

    public EventFormatViewModelValidator()
    {
        RuleFor(x => x.EventFormat)
            .NotEmpty()
            .WithMessage(EventFormatErrorMessage);
    }
}