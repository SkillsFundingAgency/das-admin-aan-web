using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Authentication;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

namespace SFA.DAS.Admin.Aan.Web.Controllers.CreateEvent;

[Authorize(Roles = Roles.ManageEventsRole)]
[Route("manage-events/new/event-format", Name = RouteNames.CreateEvent.EventFormat)]
public class NetworkEventFormatController : Controller
{
    public const string ViewPath = "~/Views/NetworkEvent/EventFormat.cshtml";
    private readonly ISessionService _sessionService;
    private readonly IValidator<CreateEventFormatViewModel> _validator;

    public NetworkEventFormatController(ISessionService sessionService, IValidator<CreateEventFormatViewModel> validator)
    {
        _sessionService = sessionService;
        _validator = validator;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var sessionModel = _sessionService.Get<CreateEventSessionModel>();
        var model = GetViewModel(sessionModel);
        return View(ViewPath, model);
    }

    [HttpPost]
    public IActionResult Post(CreateEventFormatViewModel submitModel)
    {
        var sessionModel = _sessionService.Get<CreateEventSessionModel?>() ?? new CreateEventSessionModel();

        var result = _validator.Validate(submitModel);

        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            return View(ViewPath, GetViewModel(sessionModel));
        }

        sessionModel.EventFormat = submitModel.EventFormat;
        _sessionService.Set(sessionModel);
        return RedirectToAction("Get", "NetworkEventType");
    }

    private CreateEventFormatViewModel GetViewModel(CreateEventSessionModel sessionModel)
    {
        return new CreateEventFormatViewModel
        {
            EventFormat = sessionModel?.EventFormat,
            CancelLink = Url.RouteUrl(RouteNames.NetworkEvents)!,
            PostLink = Url.RouteUrl(RouteNames.CreateEvent.EventFormat)!,
            PageTitle = Application.Constants.CreateEvent.PageTitle
        };
    }
}
