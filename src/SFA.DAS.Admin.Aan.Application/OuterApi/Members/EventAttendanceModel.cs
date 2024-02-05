namespace SFA.DAS.Admin.Aan.Application.OuterApi.Members;
public class EventAttendanceModel
{
    public Guid CalendarEventId { get; set; }
    public DateTime EventDate { get; set; }
    public string EventTitle { get; set; } = null!;
    public long? Urn { get; set; }
}
