using FluentValidation;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

public class SchoolNameViewModelValidator : AbstractValidator<SchoolNameViewModel>
{
    public const string EventSchoolNameEmpty = "You must include the name of the school";

    public SchoolNameViewModelValidator()
    {
        RuleFor(e => e.SearchTerm)
            .Must(SchoolNameEntered)
                .WithMessage(EventSchoolNameEmpty);
    }

    private static bool SchoolNameEntered(SchoolNameViewModel model, string? searchTerm)
    {
        return !string.IsNullOrEmpty(model.Urn);
    }
}