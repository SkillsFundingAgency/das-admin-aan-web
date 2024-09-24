namespace SFA.DAS.Admin.Aan.Application.OuterApi.CalendarEventAttendees
{
    public class GetCalendarEventAttendeesResponse
    {
        public List<Attendee> Attendees { get; set; } = null!;

        public class Attendee
        {
            public string Name { get; set; } = null!;
            public string Email { get; set; } = null!;
            public DateTime SignUpDate { get; set; }
        }
    }
}
