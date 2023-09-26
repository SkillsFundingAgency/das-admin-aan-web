using FluentValidation;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

namespace SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

public class EventAtSchoolViewModelValidator : AbstractValidator<EventAtSchoolViewModel>
{
    public const string EventAtSchoolEmpty = "You must make a selection";

    public EventAtSchoolViewModelValidator()
    {
        RuleFor(x => x.IsAtSchool)
            .NotEmpty()
            .WithMessage(EventAtSchoolEmpty);
    }
}