namespace SFA.DAS.Admin.Aan.Application.OuterApi.Members;
public class EventsModel
{
    public DateRangeModel EventsDateRange { get; set; } = new DateRangeModel();
    public List<EventAttendanceModel> Events { get; set; } = new List<EventAttendanceModel>();
}
