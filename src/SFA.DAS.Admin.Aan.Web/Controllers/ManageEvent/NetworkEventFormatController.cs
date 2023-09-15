using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Authentication;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

namespace SFA.DAS.Admin.Aan.Web.Controllers.ManageEvent;

[Authorize(Roles = Roles.ManageEventsRole)]
[Route("events/new/format", Name = RouteNames.ManageEvent.EventFormat)]
public class NetworkEventFormatController : Controller
{
    public const string ViewPath = "~/Views/ManageEvent/EventFormat.cshtml";
    private readonly ISessionService _sessionService;
    private readonly IValidator<EventFormatViewModel> _validator;

    public NetworkEventFormatController(ISessionService sessionService, IValidator<EventFormatViewModel> validator)
    {
        _sessionService = sessionService;
        _validator = validator;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var sessionModel = _sessionService.Get<EventSessionModel?>() ?? new EventSessionModel();
        var model = GetViewModel(sessionModel);
        return View(ViewPath, model);
    }

    [HttpPost]
    public IActionResult Post(EventFormatViewModel submitModel)
    {
        var sessionModel = _sessionService.Get<EventSessionModel?>() ?? new EventSessionModel();

        var result = _validator.Validate(submitModel);

        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            return View(ViewPath, GetViewModel(sessionModel));
        }

        sessionModel.EventFormat = submitModel.EventFormat;
        _sessionService.Set(sessionModel);
        return RedirectToRoute(RouteNames.ManageEvent.EventType);
    }

    private EventFormatViewModel GetViewModel(EventSessionModel sessionModel)
    {
        return new EventFormatViewModel
        {
            EventFormat = sessionModel?.EventFormat,
            CancelLink = Url.RouteUrl(RouteNames.NetworkEvents)!,
            PostLink = Url.RouteUrl(RouteNames.ManageEvent.EventFormat)!,
            PageTitle = Application.Constants.CreateEvent.PageTitle
        };
    }
}
