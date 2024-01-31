namespace SFA.DAS.Admin.Aan.Web.Models;

public class MemberActivitiesViewModel
{
    public List<EventViewModel> FutureEvents { get; set; } = new List<EventViewModel>();

    public int FutureEventsCount { get { return FutureEvents.Count; } }
}

public record EventViewModel(string EventTitle, string EventDate);
