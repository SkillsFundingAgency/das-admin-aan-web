using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Application.Constants;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Authentication;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.Controllers.ManageEvent;

[Authorize(Roles = Roles.ManageEventsRole)]
[Route("events/new/format", Name = RouteNames.ManageEvent.EventFormat)]
public class EventFormatController : Controller
{
    public const string ViewPath = "~/Views/ManageEvent/EventFormat.cshtml";
    private readonly ISessionService _sessionService;
    private readonly IValidator<EventFormatViewModel> _validator;

    public EventFormatController(ISessionService sessionService, IValidator<EventFormatViewModel> validator)
    {
        _sessionService = sessionService;
        _validator = validator;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();
        var model = GetViewModel(sessionModel);
        return View(ViewPath, model);
    }

    [HttpPost]
    public IActionResult Post(EventFormatViewModel submitModel)
    {
        var result = _validator.Validate(submitModel);

        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            return View(ViewPath, submitModel);
        }

        var sessionModel = _sessionService.Get<EventSessionModel>();
        sessionModel.EventFormat = submitModel.EventFormat;

        _sessionService.Set(sessionModel);
        if (sessionModel.HasSeenPreview)
        {
            return RedirectToRoute(RouteNames.ManageEvent.Location);
        }

        return RedirectToRoute(RouteNames.ManageEvent.EventType);
    }

    private EventFormatViewModel GetViewModel(EventSessionModel sessionModel)
    {
        var model = new EventFormatViewModel
        {
            EventFormat = sessionModel.EventFormat,
            CancelLink = Url.RouteUrl(RouteNames.NetworkEvents)!,
            PageTitle = CreateEvent.PageTitle
        };

        if (sessionModel!.HasSeenPreview)
        {
            model.CancelLink = Url.RouteUrl(RouteNames.ManageEvent.CheckYourAnswers)!;
        }

        model.PostLink = Url.RouteUrl(RouteNames.ManageEvent.EventFormat)!;
        return model;
    }
}