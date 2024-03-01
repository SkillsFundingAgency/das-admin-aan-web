namespace SFA.DAS.Admin.Aan.Web.Models;

public class MemberActivitiesViewModel
{
    public string FirstName { get; set; } = null!;
    public string LastEventSignUpDate { get; set; } = null!;
    public int EventsAttendedCount { get; set; }
    public int SchoolEventsAttendedCount { get; set; }
    public List<EventViewModel> FutureEvents { get; set; } = [];
    public int FutureEventsCount { get { return FutureEvents.Count; } }
}

public record EventViewModel(string EventTitle, string EventDate);
