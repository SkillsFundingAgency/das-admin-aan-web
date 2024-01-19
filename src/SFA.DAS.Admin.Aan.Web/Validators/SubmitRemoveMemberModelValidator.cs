using FluentValidation;
using SFA.DAS.Admin.Aan.Web.Models.RemoveMember;

namespace SFA.DAS.Admin.Aan.Web.Validators;

public class SubmitRemoveMemberModelValidator : AbstractValidator<SubmitRemoveMemberModel>
{
    public const string HasRemoveConfirmedValidationMessage = "You must confirm you are sure you want to remove this ambassador";
    public const string ReasonForRemoveMemberRequired = "You must tell us why you want to remove the ambassador from the network";
    public SubmitRemoveMemberModelValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty()
            .WithMessage(ReasonForRemoveMemberRequired);

        RuleFor(x => x.HasRemoveConfirmed)
            .Equal(true)
            .WithMessage(HasRemoveConfirmedValidationMessage);
    }
}
