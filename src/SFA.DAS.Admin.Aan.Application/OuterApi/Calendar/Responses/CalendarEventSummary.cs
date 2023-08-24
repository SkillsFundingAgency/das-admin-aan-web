using SFA.DAS.Admin.Aan.Application.Constants;

namespace SFA.DAS.Admin.Aan.Application.OuterApi.Calendar.Responses;

public class CalendarEventSummary
{
    public Guid CalendarEventId { get; set; }
    public bool IsActive { get; set; }
    public string Title { get; set; } = null!;
    public string CalendarName { get; set; } = null!;
    public EventFormat EventFormat { get; set; }
    public DateTime Start { get; set; }
    public int NumberOfAttendees { get; set; }
}