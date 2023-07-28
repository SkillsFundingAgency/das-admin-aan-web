namespace SFA.DAS.Admin.Aan.Domain.OuterApi.Responses;
public class GetCalendarEventsQueryResult
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public IEnumerable<CalendarEventSummary> CalendarEvents { get; set; } = Enumerable.Empty<CalendarEventSummary>();

}

public class CalendarEventSummary
{
    public Guid CalendarEventId { get; set; }
    public bool IsActive { get; set; }
    public string Title { get; set; } = null!;
    public string CalendarName { get; set; } = null!;
    public string EventFormat { get; set; } = null!;
    public DateTime Start { get; set; }
    public int NumberOfAttendees { get; set; }
}
