using FluentValidation;
using SFA.DAS.Admin.Aan.Web.Models.DeleteEvent;

namespace SFA.DAS.Admin.Aan.Web.Validators;

public class DeleteEventViewModelValidator : AbstractValidator<DeleteEventViewModel>
{
    public const string ConfirmCancelEventNotPicked = "You must confirm you want to cancel this event";

    public DeleteEventViewModelValidator()
    {
        RuleFor(x => x.IsCancelConfirmed)
            .Equal(true)
            .WithMessage(ConfirmCancelEventNotPicked);
    }
}