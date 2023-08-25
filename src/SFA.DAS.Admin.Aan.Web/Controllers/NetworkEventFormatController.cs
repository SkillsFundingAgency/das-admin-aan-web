using FluentValidation;
using FluentValidation.AspNetCore;
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
    private readonly IValidator<CreateEventFormatViewModel> _validator;

    public NetworkEventFormatController(ISessionService sessionService, IValidator<CreateEventFormatViewModel> validator)
    {
        _sessionService = sessionService;
        _validator = validator;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var sessionModel = _sessionService.Get<CreateEventSessionModel>();
        var model = GetViewModel(sessionModel);
        return View(ViewPath, model);
    }

    [HttpPost]
    public IActionResult Post(CreateEventFormatViewModel submitModel)
    {
        var sessionModel = _sessionService.Get<CreateEventSessionModel>() ?? new CreateEventSessionModel();

        var result = _validator.Validate(submitModel);

        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            return View(ViewPath, GetViewModel(sessionModel));
        }

        sessionModel.EventFormat = submitModel.EventFormat;
        _sessionService.Set(sessionModel);
        return View(ViewPath, GetViewModel(sessionModel));
    }

    private CreateEventFormatViewModel GetViewModel(CreateEventSessionModel sessionModel)
    {
        return new CreateEventFormatViewModel
        {
            EventFormat = sessionModel?.EventFormat,
            BackLink = Url.RouteUrl(RouteNames.NetworkEvents)
        };
    }
}
