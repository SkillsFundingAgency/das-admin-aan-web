using FluentValidation;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

namespace SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

public class EventLocationViewModelValidator : AbstractValidator<EventLocationViewModel>
{
    public const int EventLocationMaxCount = 200;
    public const int EventOnlineLinkMaxLength = 1000;
    public const string EventOutlineEmpty = "You must include an event location";
    public const string EventLocationTooLong = "Your event location must be 200 characters or less";

    public EventLocationViewModelValidator()
    {
        RuleFor(x => x.EventLocation)
            .NotEmpty()
            .WithMessage(EventOutlineEmpty)
            .MaximumLength(EventLocationMaxCount)
            .WithMessage(EventLocationTooLong);
    }
}