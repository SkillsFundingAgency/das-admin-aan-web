using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Aan.SharedUi.Infrastructure;
using SFA.DAS.Admin.Aan.Application.OuterApi.Members;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Authentication;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.RemoveMember;

namespace SFA.DAS.Admin.Aan.Web.Controllers.ManageMembers;

[Authorize(Roles = Roles.ManageMembersRole)]
public class RemoveMemberController : Controller
{
    private readonly ISessionService _sessionService;
    private readonly IOuterApiClient _outerApiClient;
    private readonly IValidator<SubmitRemoveMemberModel> _validator;
    public RemoveMemberController(ISessionService sessionService, IOuterApiClient outerApiClient, IValidator<SubmitRemoveMemberModel> validator)
    {
        _sessionService = sessionService;
        _outerApiClient = outerApiClient;
        _validator = validator;
    }

    [HttpGet]
    [Route("remove-member/{id}", Name = RouteNames.RemoveMember)]
    public async Task<IActionResult> Index([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        RemoveMemberViewModel removeMemberViewModel = new RemoveMemberViewModel();
        var adminMemberId = _sessionService.GetMemberId();
        var memberProfiles = await _outerApiClient.GetMemberProfile(id, adminMemberId, cancellationToken);
        removeMemberViewModel.FullName = memberProfiles.FullName;
        removeMemberViewModel.CancelLink = Url.RouteUrl(SharedRouteNames.MemberProfile, new { id = id })!;
        removeMemberViewModel.MemberId = id;
        return View(removeMemberViewModel);
    }

    [HttpPost]
    [Route("remove-member/{id}", Name = RouteNames.RemoveMember)]
    public async Task<IActionResult> Index([FromRoute] Guid id, SubmitRemoveMemberModel submitRemoveMemberModel, CancellationToken cancellationToken)
    {
        var adminMemberId = _sessionService.GetMemberId();
        var postMemberStatusModel = new PostMemberStatusModel
        {
            Status = submitRemoveMemberModel.Status
        };
        await _outerApiClient.PostMemberLeaving(id, adminMemberId, postMemberStatusModel, cancellationToken);

        return RedirectToAction("RemoveMemberConfirmation");
    }

    [HttpGet]
    public ActionResult RemoveMemberConfirmation()
    {
        RemoveMemberConfirmationModel removeMemberConfirmationModel = new RemoveMemberConfirmationModel();
        removeMemberConfirmationModel.NetworkDirectoryLink = Url.RouteUrl(SharedRouteNames.NetworkDirectory)!;
        return View(removeMemberConfirmationModel);
    }
}
