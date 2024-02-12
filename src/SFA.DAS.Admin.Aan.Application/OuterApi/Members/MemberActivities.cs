namespace SFA.DAS.Admin.Aan.Application.OuterApi.Members;
public record MemberActivities(DateTime? LastSignedUpDate, EventsModel EventsAttended, EventsModel EventsPlanned);

public record EventsModel(DateRangeModel EventsDateRange, List<EventAttendanceModel> Events);

public record DateRangeModel(DateTime FromDate, DateTime ToDate);

public record EventAttendanceModel(Guid CalendarEventId, DateTime EventDate, string EventTitle, long? Urn);
