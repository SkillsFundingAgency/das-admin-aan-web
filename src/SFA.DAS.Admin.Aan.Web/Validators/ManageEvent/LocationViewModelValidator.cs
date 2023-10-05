using FluentValidation;
using SFA.DAS.Admin.Aan.Application.Constants;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

public class LocationViewModelValidator : AbstractValidator<LocationViewModel>
{
    public const string EventLocationEmpty = "You must include an in person event location";
    public const string EventLinkMustBeValid = "The event link must be a valid url eg https://www.test.com";
    public LocationViewModelValidator()
    {
        RuleFor(e => e.SearchTerm)
                .Must(LocationVisibleButNotEntered)
                .WithMessage(EventLocationEmpty);

        RuleFor(e => e.OnlineEventLink)
            .Matches(RegularExpressions.UrlRegex)
            .WithMessage(EventLinkMustBeValid)
            .When(l => !string.IsNullOrEmpty(l.OnlineEventLink));

    }

    private static bool LocationVisibleButNotEntered(LocationViewModel model, string? searchTerm)
    {
        return !(model.ShowLocationDropdown && string.IsNullOrEmpty(model.Postcode));
    }
}