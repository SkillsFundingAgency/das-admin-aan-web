using FluentValidation;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

public class NumberOfAttendeesViewModelValidator : AbstractValidator<NumberOfAttendeesViewModel>
{

    public const string NumberOfAttendeesEmpty = "You must include the estimated number of audience members you expect at this event";

    public NumberOfAttendeesViewModelValidator()
    {
        RuleFor(x => x.NumberOfAttendees)
            .NotEmpty()
            .WithMessage(NumberOfAttendeesEmpty);
    }
}