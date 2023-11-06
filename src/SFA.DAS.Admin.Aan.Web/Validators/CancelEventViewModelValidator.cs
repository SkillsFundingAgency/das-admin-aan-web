using FluentValidation;
using SFA.DAS.Admin.Aan.Web.Models.DeleteEvent;

namespace SFA.DAS.Admin.Aan.Web.Validators;

public class CancelEventViewModelValidator : AbstractValidator<CancelEventViewModel>
{
    public const string ConfirmCancelEventNotPicked = "You must confirm you want to cancel this event";

    public CancelEventViewModelValidator()
    {
        RuleFor(x => x.IsCancelConfirmed)
            .Equal(true)
            .WithMessage(ConfirmCancelEventNotPicked);
    }
}