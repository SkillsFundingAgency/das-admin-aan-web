using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Aan.SharedUi.Constants;
using SFA.DAS.Aan.SharedUi.Infrastructure;
using SFA.DAS.Aan.SharedUi.Models.NetworkDirectory;
using SFA.DAS.Admin.Aan.Application.OuterApi.Members;
using SFA.DAS.Admin.Aan.Application.OuterApi.Regions;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Controllers.ManageMembers;
using SFA.DAS.Admin.Aan.Web.UnitTests.TestHelpers;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers.ManageMembers;

public class NetworkDirectoryControllerTests
{
    private NetworkDirectoryRequestModel _requestModel = null!;
    private IActionResult _actualResult = null!;
    private GetMembersResponse _getMembersResponse = null!;
    private GetRegionsResult _getRegionsResponse = null!;
    private List<Region> _regionsData = null!;
    private const int RegionsCount = 10;
    private string _networkDirectoryUrl = null!;

    [SetUp]
    public async Task Init()
    {
        Fixture fixture = new();
        var cancellationToken = fixture.Create<CancellationToken>();

        Mock<IOuterApiClient> apiClientMock = new();

        _getMembersResponse = fixture.Create<GetMembersResponse>();
        apiClientMock.Setup(c => c.GetMembers(It.IsAny<Dictionary<string, string[]>>(), cancellationToken)).ReturnsAsync(_getMembersResponse);

        var sequence = new Queue<int>(Enumerable.Range(1, RegionsCount));
        _regionsData = fixture.Build<Region>().With(r => r.Ordering, () => sequence.Dequeue()).CreateMany(RegionsCount).ToList();
        _getRegionsResponse = fixture.Build<GetRegionsResult>().With(r => r.Regions, _regionsData).Create();
        apiClientMock.Setup(c => c.GetRegions(cancellationToken)).ReturnsAsync(_getRegionsResponse);

        NetworkDirectoryController sut = new(apiClientMock.Object);
        _networkDirectoryUrl = fixture.Create<string>();
        sut.AddUrlHelperMock().AddUrlForRoute(SharedRouteNames.NetworkDirectory, _networkDirectoryUrl);

        _requestModel = fixture.Build<NetworkDirectoryRequestModel>()
            .With(m => m.RegionId, _regionsData.Take(2).Select(r => r.Id).ToList())
            .With(m => m.UserRole, new List<Role>() { Role.Apprentice })
            .Create();

        _actualResult = await sut.Index(_requestModel, cancellationToken);
    }

    [Test]
    public void ThenReturnsViewResult()
    {
        _actualResult.Should().BeOfType<ViewResult>();
    }

    [Test]
    public void ThenReturnsNetworkDirectoryViewModel()
    {
        _actualResult.As<ViewResult>().Model.Should().BeOfType<NetworkDirectoryViewModel>();
        _actualResult.As<ViewResult>().Model.As<NetworkDirectoryViewModel>().TotalCount.Should().Be(_getMembersResponse.TotalCount);
    }

    [Test]
    public void ThenReturnsNetworkDirectoryViewModelWithMembers()
    {
        _actualResult.As<ViewResult>().Model.Should().BeOfType<NetworkDirectoryViewModel>();
        _actualResult.As<ViewResult>().Model.As<NetworkDirectoryViewModel>().Members.Count.Should().Be(_getMembersResponse.Members.Count());
        _actualResult.As<ViewResult>().Model.As<NetworkDirectoryViewModel>().Members.Should().Contain(m => m.MemberProfileLink == "#");
    }

    [Test]
    public void ThenNetworkDirectoryViewModelHasPaginationViewModel()
    {
        var model = _actualResult.As<ViewResult>().Model.As<NetworkDirectoryViewModel>();

        using (new AssertionScope())
        {
            model.PaginationViewModel.Should().NotBeNull();
            model.PaginationViewModel.CurrentPage.Should().Be(_getMembersResponse.Page);
            model.PaginationViewModel.PageSize.Should().Be(_getMembersResponse.PageSize);
            model.PaginationViewModel.TotalPages.Should().Be(_getMembersResponse.TotalPages);
        }
    }

    [Test]
    public void ThenNetworkDirectoryViewModelHasDirectoryFilterChoices()
    {
        var model = _actualResult.As<ViewResult>().Model.As<NetworkDirectoryViewModel>();

        using (new AssertionScope("FilterChoices"))
        {
            model.FilterChoices.Should().NotBeNull();
            model.FilterChoices.Keyword.Should().Be(_requestModel.Keyword?.Trim());
        }
    }

    [Test]
    public void ThenNetworkDirectoryViewModelHasRoleFilterChoices()
    {
        var model = _actualResult.As<ViewResult>().Model.As<NetworkDirectoryViewModel>();
        using (new AssertionScope("RoleCheckList"))
        {
            model.FilterChoices.RoleChecklistDetails.Title.Should().Be(NetworkDirectoryController.RoleCheckListTitle);
            model.FilterChoices.RoleChecklistDetails.QueryStringParameterName.Should().Be(NetworkDirectoryController.RoleCheckListQueryParameterName);
            model.FilterChoices.RoleChecklistDetails.Lookups.Count().Should().Be(3);
            model.FilterChoices.RoleChecklistDetails.Lookups.Should().Contain(l => l.Value == nameof(Role.Apprentice) && l.Checked.Equals("checked"));
            model.FilterChoices.RoleChecklistDetails.Lookups.Should().Contain(l => l.Value == nameof(Role.Employer) && l.Checked.Equals(string.Empty));
            model.FilterChoices.RoleChecklistDetails.Lookups.Should().Contain(l => l.Value == nameof(Role.RegionalChair) && l.Checked.Equals(string.Empty));
        }
    }

    [Test]
    public void ThenNetworkDirectoryViewModelHasRegionFilterChoices()
    {
        var model = _actualResult.As<ViewResult>().Model.As<NetworkDirectoryViewModel>();
        using (new AssertionScope("RegionsCheckList"))
        {
            model.FilterChoices.RegionChecklistDetails.Title.Should().Be(NetworkDirectoryController.RegionCheckListTitle);
            model.FilterChoices.RegionChecklistDetails.QueryStringParameterName.Should().Be(NetworkDirectoryController.RegionCheckListParameterName);
            model.FilterChoices.RegionChecklistDetails.Lookups.Count().Should().Be(RegionsCount + 1);
            model.FilterChoices.RegionChecklistDetails.Lookups.Where(l => l.Checked == "checked").Count().Should().Be(2);
            model.FilterChoices.RegionChecklistDetails.Lookups.Where(l => l.Checked == string.Empty).Count().Should().Be(9);
            model.FilterChoices.RegionChecklistDetails.Lookups.Should().Contain(r => r.Value == "0" && r.Name == "Multi-regional");
        }
    }

    [Test]
    public void ThenNetworkDirectoryViewModelHasSelectedFilters()
    {
        var model = _actualResult.As<ViewResult>().Model.As<NetworkDirectoryViewModel>();

        using (new AssertionScope("Selected filters"))
        {
            model.SelectedFiltersModel.SelectedFilters.Should().NotBeNullOrEmpty();
            model.SelectedFiltersModel.SelectedFilters.Should().ContainSingle(s => s.FieldName == "Role" && s.Filters.Any(f => f.Value == "Apprentice"));
            model.SelectedFiltersModel.SelectedFilters.Should().ContainSingle(s => s.FieldName == "Network directory" && s.Filters.Any(f => f.Value == _requestModel.Keyword));
            model.SelectedFiltersModel.SelectedFilters.Should().ContainSingle(s => s.FieldName == "Region" && s.Filters.Count == 2);
            model.SelectedFiltersModel.ClearSelectedFiltersLink.Should().Be(_networkDirectoryUrl);
        }
    }
}
