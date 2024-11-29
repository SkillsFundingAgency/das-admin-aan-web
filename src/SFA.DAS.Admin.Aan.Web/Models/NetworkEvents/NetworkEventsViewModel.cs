using Microsoft.AspNetCore.Mvc.Rendering;
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
    public bool ShowCalendarEvents => CalendarEvents.Any();
    public string ClearSelectedFiltersLink { get; set; } = null!;
    public string CreateEventLink { get; set; } = null!;
    public string OrderBy { get; set; }
    public List<SelectListItem> OrderByOptions => new List<SelectListItem>
        { new("Soonest", "soonest"), new("Closest", "closest") };
    public bool ShowSortOptions => !string.IsNullOrWhiteSpace(FilterChoices.Location) && CalendarEvents.Any();
    public bool IsInvalidLocation { get; set; }
    public string SearchedLocation { get; set; } = string.Empty;
}