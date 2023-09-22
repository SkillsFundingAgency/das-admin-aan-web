using FluentValidation;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

namespace SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

public class EventLocationViewModelValidator : AbstractValidator<EventLocationViewModel>
{
    public const string EventLocationEmpty = "You must include an event location";

    public EventLocationViewModelValidator()
    {
        When(m => string.IsNullOrWhiteSpace(m.Postcode), () =>
        {
            RuleFor(e => e.SearchTerm)
                .NotEmpty()
                .WithMessage(EventLocationEmpty);
        });
    }
}