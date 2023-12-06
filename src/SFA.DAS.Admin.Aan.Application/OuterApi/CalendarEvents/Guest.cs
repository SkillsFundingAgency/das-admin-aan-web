namespace SFA.DAS.Admin.Aan.Application.OuterApi.CalendarEvents;

public class Guest
{
    public Guest(string name, string jobTitle)
    {
        GuestName = name;
        GuestJobTitle = jobTitle;
    }

    public string GuestName { get; set; }
    public string GuestJobTitle { get; set; }
}