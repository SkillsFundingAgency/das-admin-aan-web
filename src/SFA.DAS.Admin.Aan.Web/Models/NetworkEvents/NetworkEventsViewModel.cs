namespace SFA.DAS.Admin.Aan.Web.Models.NetworkEvents;

public class NetworkEventsViewModel
{
    public PaginationViewModel PaginationViewModel { get; set; } = null!;
    public int TotalCount { get; set; }
    public List<CalendarEventViewModel> CalendarEvents { get; set; } = new List<CalendarEventViewModel>();
    public EventFilterChoices FilterChoices { get; set; } = new EventFilterChoices();
    public List<SelectedFilter> SelectedFilters { get; set; } = new List<SelectedFilter>();
    public bool ShowFilterOptions => SelectedFilters.Any();
    public string ClearSelectedFiltersLink { get; set; } = null!;
}