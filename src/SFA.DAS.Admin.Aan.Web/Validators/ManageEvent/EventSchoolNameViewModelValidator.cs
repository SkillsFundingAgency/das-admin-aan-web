using FluentValidation;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

namespace SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

public class EventSchoolNameViewModelValidator : AbstractValidator<EventSchoolNameViewModel>
{
    public const string EventSchoolNameEmpty = "You must include the name of the school";

    public EventSchoolNameViewModelValidator()
    {
        RuleFor(e => e.SearchTerm)
            .Must(SchoolNotEntered)
                .WithMessage(EventSchoolNameEmpty);
    }

    private static bool SchoolNotEntered(EventSchoolNameViewModel model, string? searchTerm)
    {
        return !(string.IsNullOrEmpty(model.Urn));
    }
}