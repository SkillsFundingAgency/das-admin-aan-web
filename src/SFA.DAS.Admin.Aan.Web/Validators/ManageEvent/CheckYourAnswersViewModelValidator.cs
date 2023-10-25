using FluentValidation;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

public class CheckYourAnswersViewModelValidator : AbstractValidator<CheckYourAnswersViewModel>
{
    public const string EventFormatHasLocationAndLocationEmpty = "You must include an in person event location";
    public const string EventSchoolNameEmpty = "You must include the name of the school";

    public CheckYourAnswersViewModelValidator()
    {
        RuleFor(e => e.EventLocation)
            .Must(EventFormatRequiredAndPresent)
            .WithMessage(EventFormatHasLocationAndLocationEmpty);

        RuleFor(e => e.SchoolName)
            .Must(SchoolNameRequired)
            .WithMessage(EventSchoolNameEmpty);
    }

    private static bool SchoolNameRequired(CheckYourAnswersViewModel model, string? schoolName)
    {
        if (model.IsAtSchool.GetValueOrDefault()) return !string.IsNullOrEmpty(schoolName);
        return true;
    }

    private static bool EventFormatRequiredAndPresent(CheckYourAnswersViewModel model, string? eventLocation)
    {
        return !(model.ShowLocation && string.IsNullOrEmpty(eventLocation?.Trim()));
    }

}