using FluentAssertions;
using SFA.DAS.Admin.Aan.Web.Models;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvents;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Models;
[TestFixture]
public class NetworkEventsViewModelTests
{
    [TestCase(true, true, 1, 3)]
    [TestCase(false, false, 2, 4)]
    public void ShowFilterOptions_ReturningExpectedValueFromParameters(bool searchFilterAdded, bool expected, int totalCount, int pageSize)
    {
        var paginationViewModel = new PaginationViewModel(1, pageSize, 5, "test");
        var model = new NetworkEventsViewModel()
        {
            TotalCount = totalCount,
            SelectedFilters = new List<SelectedFilter>(),
            PaginationViewModel = paginationViewModel
        };

        if (searchFilterAdded)
        {
            model.SelectedFilters.Add(new SelectedFilter());
        }

        var actual = model.ShowFilterOptions;
        actual.Should().Be(expected);
        model.TotalCount.Should().Be(totalCount);
        model.PaginationViewModel.Should().BeEquivalentTo(paginationViewModel);
    }
}