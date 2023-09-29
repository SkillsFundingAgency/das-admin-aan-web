using FluentValidation;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

public class IsAtSchoolViewModelValidator : AbstractValidator<IsAtSchoolViewModel>
{
    public const string EventAtSchoolEmpty = "You must make a selection";

    public IsAtSchoolViewModelValidator()
    {
        RuleFor(x => x.IsAtSchool)
            .NotEmpty()
            .WithMessage(EventAtSchoolEmpty);
    }
}