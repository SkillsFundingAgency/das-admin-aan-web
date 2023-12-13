using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Aan.SharedUi.Constants;
using SFA.DAS.Aan.SharedUi.Extensions;
using SFA.DAS.Aan.SharedUi.Infrastructure;
using SFA.DAS.Aan.SharedUi.Models;
using SFA.DAS.Aan.SharedUi.Models.NetworkDirectory;
using SFA.DAS.Aan.SharedUi.Models.NetworkEvents;
using SFA.DAS.Aan.SharedUi.Services;
using SFA.DAS.Admin.Aan.Application.OuterApi.Members;
using SFA.DAS.Admin.Aan.Application.OuterApi.Regions;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Authentication;
using SFA.DAS.ApprenticeAan.Web.Models.NetworkDirectory;

namespace SFA.DAS.Admin.Aan.Web.Controllers.ManageMembers;

[Authorize(Roles = Roles.ManageMembersRole)]
[Route("network-directory", Name = SharedRouteNames.NetworkDirectory)]
public class NetworkDirectoryController : Controller
{
    public const string RoleCheckListTitle = "Role";
    public const string RoleCheckListQueryParameterName = "userRole";
    public const string RegionCheckListTitle = "Region";
    public const string RegionCheckListParameterName = "regionId";
    private readonly IOuterApiClient _outerApiClient;

    public NetworkDirectoryController(IOuterApiClient outerApiClient)
    {
        _outerApiClient = outerApiClient;
    }

    public async Task<IActionResult> Index(NetworkDirectoryRequestModel request, CancellationToken cancellationToken)
    {
        var getMembersTask = _outerApiClient.GetMembers(request.ToQueryStringParameters(), cancellationToken);
        var regionTask = _outerApiClient.GetRegions(cancellationToken);

        await Task.WhenAll(getMembersTask, regionTask);

        var regions = regionTask.Result.Regions;
        regions.Add(new Region(0, "Multi-regional", regions.Count + 1));

        var model = InitialiseViewModel(getMembersTask.Result);

        var filterUrl = FilterBuilder.BuildFullQueryString(request, () => Url.RouteUrl(SharedRouteNames.NetworkDirectory)!);

        model.PaginationViewModel = new PaginationViewModel(getMembersTask.Result.Page, getMembersTask.Result.PageSize, getMembersTask.Result.TotalPages, filterUrl);

        var filterChoices = PopulateFilterChoices(request, regions);
        model.FilterChoices = filterChoices;
        model.SelectedFiltersModel.SelectedFilters = FilterBuilder.Build(request, () => Url.RouteUrl(SharedRouteNames.NetworkDirectory)!, filterChoices.RoleChecklistDetails.Lookups, filterChoices.RegionChecklistDetails.Lookups);
        model.SelectedFiltersModel.ClearSelectedFiltersLink = Url.RouteUrl(SharedRouteNames.NetworkDirectory)!;

        return View(model);
    }

    private NetworkDirectoryViewModel InitialiseViewModel(GetMembersResponse result)
    {
        var model = new NetworkDirectoryViewModel
        {
            TotalCount = result.TotalCount
        };

        foreach (var member in result.Members)
        {
            MembersViewModel vm = member;
            vm.MemberProfileLink = Url.RouteUrl(SharedRouteNames.MemberProfile, new { id = member.MemberId })!;
            model.Members.Add(vm);
        }

        return model;
    }

    private static DirectoryFilterChoices PopulateFilterChoices(NetworkDirectoryRequestModel request, List<Region> regions)
        => new()
        {
            Keyword = request.Keyword?.Trim(),
            RoleChecklistDetails = new ChecklistDetails
            {
                Title = RoleCheckListTitle,
                QueryStringParameterName = RoleCheckListQueryParameterName,
                Lookups = new ChecklistLookup[]
                {
                    new(Role.Apprentice.GetDescription(), Role.Apprentice.ToString(), request.UserRole.Exists(x => x == Role.Apprentice)),
                    new(Role.Employer.GetDescription(), Role.Employer.ToString(), request.UserRole.Exists(x => x == Role.Employer)),
                    new(Role.RegionalChair.GetDescription(), Role.RegionalChair.ToString(), request.UserRole.Exists(x => x == Role.RegionalChair))
                }
            },
            RegionChecklistDetails = new ChecklistDetails
            {
                Title = RegionCheckListTitle,
                QueryStringParameterName = RegionCheckListParameterName,
                Lookups = regions.OrderBy(x => x.Ordering).Select(region => new ChecklistLookup(region.Area, region.Id.ToString(), request.RegionId.Exists(x => x == region.Id))).ToList()
            }
        };
}
