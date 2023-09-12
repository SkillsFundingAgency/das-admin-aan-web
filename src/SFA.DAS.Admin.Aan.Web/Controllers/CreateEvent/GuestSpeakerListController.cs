

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

namespace SFA.DAS.Admin.Aan.Web.Controllers.CreateEvent;

[Authorize]
[Route("manage-events/new/guest-speaker-list", Name = RouteNames.CreateEvent.GuestSpeakerList)]
public class GuestSpeakerListController : Controller
{
    private readonly ISessionService _sessionService;

    public const string ViewPath = "~/Views/NetworkEvent/GuestSpeakerList.cshtml";
    public GuestSpeakerListController(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var sessionModel = _sessionService.Get<CreateEventSessionModel>();
        var model = GetViewModel(sessionModel);
        return View(ViewPath, model);
    }

    [HttpPost]
    public IActionResult Post()
    {
        return RedirectToAction("Get", "GuestSpeakerList");
    }


    private CreateEventGuestSpeakerListViewModel GetViewModel(CreateEventSessionModel sessionModel)
    {
        return new CreateEventGuestSpeakerListViewModel
        {
            GuestSpeakers = sessionModel.GuestSpeakers,
            CancelLink = Url.RouteUrl(RouteNames.NetworkEvents)!,
            PostLink = Url.RouteUrl(RouteNames.CreateEvent.GuestSpeakerList)!,
            AddGuestSpeakerLink = Url.RouteUrl(RouteNames.CreateEvent.GuestSpeakerAdd)!,
            DeleteSpeakerLink = Url.RouteUrl(RouteNames.CreateEvent.GuestSpeakerDelete)!,
            PageTitle = Application.Constants.CreateEvent.PageTitle
        };
    }
}
