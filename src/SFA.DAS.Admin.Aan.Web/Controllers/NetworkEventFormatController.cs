using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

namespace SFA.DAS.Admin.Aan.Web.Controllers;

[Authorize]
[Route("network-event/create-event-format", Name = RouteNames.CreateEvent.EventFormat)]
public class NetworkEventFormatController : Controller
{
    public const string ViewPath = "~/Views/NetworkEvent/EventFormat.cshtml";
    private readonly ISessionService _sessionService;

    public NetworkEventFormatController(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    public async Task<IActionResult> Get()
    {
        var model = GetViewModel();
        return View(ViewPath, model);
    }

    private CreateEventFormatViewModel GetViewModel()
    {
        var sessionModel = _sessionService.Get<CreateEventSessionModel>();

        return new CreateEventFormatViewModel
        {
            EventFormat = sessionModel?.EventFormat,
            BackLink = Url.RouteUrl(RouteNames.NetworkEvents)
        };
    }
}
