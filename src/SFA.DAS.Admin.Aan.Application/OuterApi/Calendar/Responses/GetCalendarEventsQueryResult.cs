namespace SFA.DAS.Admin.Aan.Application.OuterApi.Calendar.Responses;
public class GetCalendarEventsQueryResult
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public IEnumerable<CalendarEventSummary> CalendarEvents { get; set; } = [];

    public bool IsInvalidLocation { get; set; }

    public List<Regions.Region> Regions { get; set; } = [];
    public List<CalendarDetail> Calendars { get; set; } = [];
}