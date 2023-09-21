using FluentValidation;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

namespace SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

public class EventDateTimeViewModelValidator : AbstractValidator<EventDateTimeViewModel>
{
    public const string EventDateInPast = "Event date must not be in the past";
    public const string EventDateEmpty = "You must select an event date";
    public const string EventStartHourEmpty = "You must select a start time hour";
    public const string EventStartMinutesEmpty = "You must select a start time minutes";
    public const string EventStartHourAndMinutesEmpty = "You must select a start time hour and minutes";
    public const string EventEndHourEmpty = "You must select an end time hours";
    public const string EventEndMinutesEmpty = "You must select an end time minutes";
    public const string EventEndHourAndMinutesEmpty = "You must select an end time hour and minutes";
    public const string EventEndTimeBeforeStartTime = "Event end time is before event start time";

    public EventDateTimeViewModelValidator()
    {
        RuleFor(x => x.DateOfEvent)
            .NotEmpty()
            .WithMessage(EventDateEmpty)
            .GreaterThanOrEqualTo(DateTime.Today)
            .WithMessage(EventDateInPast);

        RuleFor(dateTime => dateTime.StartHour)
             .Cascade(CascadeMode.Stop)
             .Must(StartHoursAndMinutesPresent)
             .WithMessage(EventStartHourAndMinutesEmpty)
             .NotEmpty()
             .WithMessage(EventStartHourEmpty)
             .Must(StartTimePresent)
             .WithMessage(EventStartMinutesEmpty);

        RuleFor(dateTime => dateTime.EndHour)
            .Cascade(CascadeMode.Stop)
            .Must(EndHoursAndMinutesPresent)
            .WithMessage(EventEndHourAndMinutesEmpty)
            .NotEmpty()
            .WithMessage(EventEndHourEmpty)
            .Must(EndTimePresent)
            .WithMessage(EventEndMinutesEmpty)
            .Must(StartTimeBeforeEndTime)
            .WithMessage(EventEndTimeBeforeStartTime);
    }

    private static bool StartHoursAndMinutesPresent(EventDateTimeViewModel model, int? startHour)
    {
        return !(model.StartMinutes == null && startHour == null);
    }

    private static bool StartTimePresent(EventDateTimeViewModel model, int? startHour)
    {
        return model.StartMinutes != null;
    }

    private static bool EndHoursAndMinutesPresent(EventDateTimeViewModel model, int? endHour)
    {
        return !(model.EndMinutes == null && endHour == null);
    }

    private static bool EndTimePresent(EventDateTimeViewModel model, int? endHour)
    {
        return model.EndMinutes != null;
    }

    private static bool StartTimeBeforeEndTime(EventDateTimeViewModel model, int? startHour)
    {
        if (model.StartHour == null || model.StartMinutes == null || model.EndHour == null ||
            model.EndMinutes == null) return true;

        var startTime = (int)((model.StartHour * 60) + model.StartMinutes);
        var endTime = (int)((model.EndHour * 60) + model.EndMinutes);
        return startTime <= endTime;
    }
}