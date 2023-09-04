using FluentValidation;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

namespace SFA.DAS.Admin.Aan.Web.Validators.CreateEvent;

public class CreateEventFormatViewModelValidator : AbstractValidator<CreateEventFormatViewModel>
{
    public const string EventFormatErrorMessage = "You must select an event format";

    public CreateEventFormatViewModelValidator()
    {
        RuleFor(x => x.EventFormat)
            .NotEmpty()
            .WithMessage(EventFormatErrorMessage);
    }
}
