using FluentValidation;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

public class EventFormatViewModelValidator : AbstractValidator<ManageEventFormatViewModel>
{
    public const string EventFormatErrorMessage = "You must select an event format";

    public EventFormatViewModelValidator()
    {
        RuleFor(x => x.EventFormat)
            .NotEmpty()
            .WithMessage(EventFormatErrorMessage);
    }
}