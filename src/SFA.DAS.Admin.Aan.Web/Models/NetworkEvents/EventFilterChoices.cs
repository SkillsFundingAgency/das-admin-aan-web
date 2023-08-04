namespace SFA.DAS.Admin.Aan.Web.Models.NetworkEvents;

public class EventFilterChoices
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public ChecklistDetails EventStatusChecklistDetails { get; set; } = new ChecklistDetails();
    public ChecklistDetails EventTypeChecklistDetails { get; set; } = new ChecklistDetails();

}
