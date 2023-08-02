using SFA.DAS.Admin.Aan.Domain.Constants;
using SFA.DAS.Admin.Aan.Domain.OuterApi.Responses;

namespace SFA.DAS.Admin.Aan.Web.Models.NetworkEvents;

public class NetworkEventsViewModel
{
    public PaginationViewModel PaginationViewModel { get; set; } = null!;
    public int TotalCount { get; set; }
    public List<CalendarEventViewModel> CalendarEvents { get; set; } = new List<CalendarEventViewModel>();
    public EventFilterChoices FilterChoices { get; set; } = new EventFilterChoices();
    public List<SelectedFilter> SelectedFilters { get; set; } = new List<SelectedFilter>();
    public bool ShowFilterOptions => SelectedFilters.Any();
}

public class CalendarEventViewModel
{
    public Guid CalendarEventId { get; set; }
    public string CalendarName { get; set; } = null!;

    public DateTime Start { get; set; }
    public string Title { get; set; } = null!;
    public EventFormat EventFormat { get; set; }

    public bool IsActive { get; set; }

    public string Status => IsActive ? EventStatus.Published : EventStatus.Cancelled;

    public int NumberOfAttendees { get; set; }


    public static implicit operator CalendarEventViewModel(CalendarEventSummary source)
        => new()
        {
            CalendarEventId = source.CalendarEventId,
            CalendarName = source.CalendarName,
            Start = source.Start,
            Title = source.Title,
            EventFormat = source.EventFormat,
            IsActive = source.IsActive,
            NumberOfAttendees = source.NumberOfAttendees,
        };
}
