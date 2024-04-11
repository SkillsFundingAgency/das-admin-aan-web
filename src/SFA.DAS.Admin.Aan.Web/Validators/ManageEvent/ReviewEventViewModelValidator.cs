using FluentValidation;
using SFA.DAS.Admin.Aan.Application.Constants;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

public class ReviewEventViewModelValidator : AbstractValidator<ReviewEventViewModel>
{
    public const string EventFormatHasLocationAndLocationEmpty = "You must include an in person event location";
    public const string EventSchoolNameEmpty = "You must include the name of the school";
    public const string EventLinkMustBeValid = "The event link must be a valid URL, for example https://www.test.com";
    public const string EventLinkMustNotHaveHtmlTags = "The event link must be a valid URL without the characters <,>";

    public ReviewEventViewModelValidator()
    {
        RuleFor(e => e.EventLocation)
            .Must(EventFormatRequiredAndPresent)
            .WithMessage(EventFormatHasLocationAndLocationEmpty);

        RuleFor(e => e.SchoolName)
            .Must(SchoolNameRequired)
            .WithMessage(EventSchoolNameEmpty);

        RuleFor(e => e.OnlineEventLink)
            .Cascade(CascadeMode.Stop)
            .Matches(RegularExpressions.UrlRegex)
            .WithMessage(EventLinkMustBeValid)
            .Must(LocationWithHtmlTags)
            .WithMessage(EventLinkMustNotHaveHtmlTags)
            .When(l => !string.IsNullOrEmpty(l.OnlineEventLink) && l.ShowOnlineEventLink);
    }


    private static bool SchoolNameRequired(ReviewEventViewModel model, string? schoolName)
    {
        if (model.IsAtSchool.GetValueOrDefault()) return !string.IsNullOrEmpty(schoolName);
        return true;
    }

    private static bool EventFormatRequiredAndPresent(ReviewEventViewModel model, string? eventLocation)
    {
        return !(model.ShowLocation && string.IsNullOrEmpty(eventLocation?.Trim()));
    }

    private static bool LocationWithHtmlTags(ReviewEventViewModel model, string? eventLink)
    {
        return !(model.OnlineEventLink != null && (model.OnlineEventLink.Contains('<') || model.OnlineEventLink.Contains('>')));
    }
}