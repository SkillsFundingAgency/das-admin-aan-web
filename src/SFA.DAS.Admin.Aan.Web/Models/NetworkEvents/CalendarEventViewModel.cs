using SFA.DAS.Aan.SharedUi.Constants;
using SFA.DAS.Admin.Aan.Application.Constants;
using SFA.DAS.Admin.Aan.Application.OuterApi.Calendar.Responses;

namespace SFA.DAS.Admin.Aan.Web.Models.NetworkEvents;

public class CalendarEventViewModel
{
    public Guid CalendarEventId { get; set; }
    public string CalendarName { get; set; } = null!;
    public DateTime Start { get; set; }
    public string Title { get; set; } = null!;
    public EventFormat EventFormat { get; set; }
    public bool IsActive { get; set; }
    public string Status => IsActive ? EventStatus.Published : EventStatus.Cancelled;
    public bool IsEditable => IsActive && Start >= DateTime.Now;
    public int NumberOfAttendees { get; set; }
    public string? CancelEventLink { get; set; }
    public string? EditEventLink { get; set; }
    public string? ViewDetailsLink { get; set; }

    public static implicit operator CalendarEventViewModel(CalendarEventSummary source)
        => new()
        {
            CalendarEventId = source.CalendarEventId,
            CalendarName = source.CalendarName,
            Start = source.Start,
            Title = source.Title,
            EventFormat = source.EventFormat,
            IsActive = source.IsActive,
            NumberOfAttendees = source.NumberOfAttendees
        };
}