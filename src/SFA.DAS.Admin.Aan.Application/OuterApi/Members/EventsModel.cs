namespace SFA.DAS.Admin.Aan.Application.OuterApi.Members;
public class EventsModel
{
    public DateRangeModel EventsDateRange { get; set; } = new();
    public List<EventAttendanceModel> Events { get; set; } = [];
}
