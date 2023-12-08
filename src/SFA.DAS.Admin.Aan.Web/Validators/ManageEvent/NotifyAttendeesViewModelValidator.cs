using FluentValidation;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

public class NotifyAttendeesViewModelValidator : AbstractValidator<NotifyAttendeesViewModel>
{
    public const string NotifyAttendeesErrorMessage = "You must make a selection";

    public NotifyAttendeesViewModelValidator()
    {
        RuleFor(x => x.IsNotifyAttendees)
            .NotEmpty()
            .WithMessage(NotifyAttendeesErrorMessage);
    }
}
