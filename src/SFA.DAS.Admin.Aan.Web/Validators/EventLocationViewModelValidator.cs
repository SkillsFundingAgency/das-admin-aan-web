using FluentValidation;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

namespace SFA.DAS.Admin.Aan.Web.Validators;

public class EventLocationViewModelValidator : AbstractValidator<EventLocationViewModel>
{
    public const string EventLocationEmpty = "You must include an in person event location";

    public EventLocationViewModelValidator()
    {
        RuleFor(e => e.SearchTerm)
                .Must(LocationVisibleButNotEntered)
                .WithMessage(EventLocationEmpty);

    }

    private static bool LocationVisibleButNotEntered(EventLocationViewModel model, string? searchTerm)
    {
        return !(model.ShowLocationDropdown && string.IsNullOrEmpty(model.Postcode));
    }
}