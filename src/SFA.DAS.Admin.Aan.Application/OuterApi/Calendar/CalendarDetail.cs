namespace SFA.DAS.Admin.Aan.Application.OuterApi.Calendar;

public class CalendarDetail
{
    public int Id { get; set; }
    public string CalendarName { get; set; } = string.Empty;
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public int Ordering { get; set; }
}