using FluentValidation;
using SFA.DAS.Admin.Aan.Application.Constants;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

public class LocationViewModelValidator : AbstractValidator<LocationViewModel>
{
    public const string EventLocationEmpty = "You must include an in person event location";
    public const string EventLinkMustBeValid = "The event link must be a valid URL, for example https://www.test.com";
    public const string EventLinkMustNotHaveHtmlTags = "The event link must be a valid URL without the characters <,>";

    public LocationViewModelValidator()
    {
        RuleFor(e => e.SearchTerm)
                .Must(LocationVisibleButNotEntered)
                .WithMessage(EventLocationEmpty);

        RuleFor(e => e.OnlineEventLink)
            .Cascade(CascadeMode.Stop)
            .Matches(RegularExpressions.UrlRegex)
            .WithMessage(EventLinkMustBeValid)
            .Must(LocationWithHtmlTags)
            .WithMessage(EventLinkMustNotHaveHtmlTags)
            .When(l => !string.IsNullOrEmpty(l.OnlineEventLink) && l.ShowOnlineEventLink);
    }

    private static bool LocationVisibleButNotEntered(LocationViewModel model, string? searchTerm)
    {
        return !(model.ShowLocationDropdown && string.IsNullOrEmpty(model.Postcode));
    }

    private static bool LocationWithHtmlTags(LocationViewModel model, string? eventLink)
    {
        return !(model.OnlineEventLink != null && (model.OnlineEventLink.Contains('<') || model.OnlineEventLink.Contains('>')));
    }
}