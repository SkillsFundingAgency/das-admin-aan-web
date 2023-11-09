namespace SFA.DAS.Admin.Aan.Web.Models.DeleteEvent;

public class CancelEventViewModel
{
    public Guid CalendarEventId { get; set; }
    public string Title { get; set; } = null!;

    public string? PostLink { get; set; }
    public string? ManageEventsLink { get; set; }
    public bool IsCancelConfirmed { get; set; }
}