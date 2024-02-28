namespace SFA.DAS.Admin.Aan.Application.OuterApi.Members;
public class MemberActivities
{
    public DateTime? LastSignedUpDate { get; set; }
    public EventsModel EventsAttended { get; set; } = new();
    public EventsModel EventsPlanned { get; set; } = new();
}

