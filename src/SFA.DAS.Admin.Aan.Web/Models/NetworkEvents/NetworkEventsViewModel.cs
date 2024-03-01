using SFA.DAS.Aan.SharedUi.Models;

namespace SFA.DAS.Admin.Aan.Web.Models.NetworkEvents;

public class NetworkEventsViewModel
{
    public PaginationViewModel PaginationViewModel { get; set; } = null!;
    public int TotalCount { get; set; }
    public List<CalendarEventViewModel> CalendarEvents { get; set; } = [];
    public EventFilterChoices FilterChoices { get; set; } = new();
    public List<SelectedFilter> SelectedFilters { get; set; } = [];
    public bool ShowFilterOptions => SelectedFilters.Any();
    public string ClearSelectedFiltersLink { get; set; } = null!;

    public string CreateEventLink { get; set; } = null!;
}