using FluentValidation;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

namespace SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

public class EventLocationViewModelValidator : AbstractValidator<EventLocationViewModel>
{
    public const int EventOnlineLinkMaxLength = 1000;
    public const string EventLocationEmpty = "You must include an event location";
    public const string EventOnlineLinkTooLong = "Your online event link must be 1000 characters or less";

    public EventLocationViewModelValidator()
    {
        When(m => string.IsNullOrWhiteSpace(m.Postcode), () =>
        {
            RuleFor(e => e.SearchTerm)
                .NotEmpty()
                .WithMessage(EventLocationEmpty);
        });

        RuleFor(x => x.OnlineEventLink)
            .MaximumLength(EventOnlineLinkMaxLength)
            .WithMessage(EventOnlineLinkTooLong);
    }
}