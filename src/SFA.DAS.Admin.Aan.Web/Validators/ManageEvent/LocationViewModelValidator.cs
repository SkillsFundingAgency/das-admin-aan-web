using FluentValidation;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

public class LocationViewModelValidator : AbstractValidator<LocationViewModel>
{
    public const string EventLocationEmpty = "You must include an in person event location";

    public LocationViewModelValidator()
    {
        RuleFor(e => e.SearchTerm)
                .Must(LocationVisibleButNotEntered)
                .WithMessage(EventLocationEmpty);

    }

    private static bool LocationVisibleButNotEntered(LocationViewModel model, string? searchTerm)
    {
        return !(model.ShowLocationDropdown && string.IsNullOrEmpty(model.Postcode));
    }
}