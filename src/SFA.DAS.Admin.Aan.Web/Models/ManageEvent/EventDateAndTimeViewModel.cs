namespace SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

public class EventDateAndTimeViewModel : EventPageEditFields
{
    public DateTime? DateOfEvent { get; set; }
    public int? StartHour { get; set; }
    public int? StartMinutes { get; set; }
    public int? EndHour { get; set; }
    public int? EndMinutes { get; set; }
}