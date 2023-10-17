using FluentValidation;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

public class CheckYourAnswersViewModelValidator : AbstractValidator<CheckYourAnswersViewModel>
{
    public const string EventFormatHasLocationAndLocationEmpty = "You must include an in person event location";

    public CheckYourAnswersViewModelValidator()
    {
        RuleFor(e => e.EventLocation)
            .Must(EventFormatRequiredAndPresent)
            .WithMessage(EventFormatHasLocationAndLocationEmpty);
    }

    private static bool EventFormatRequiredAndPresent(CheckYourAnswersViewModel model, string? eventLocation)
    {
        return !(model.ShowLocation && string.IsNullOrEmpty(eventLocation?.Trim()));
    }

}