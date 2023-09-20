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
[Route("events/new/datetime", Name = RouteNames.ManageEvent.EventDateTime)]
public class NetworkEventDateTimeController : Controller
{
    private readonly ISessionService _sessionService;
    private readonly IValidator<EventDateTimeViewModel> _validator;

    public const string ViewPath = "~/Views/ManageEvent/EventDateTime.cshtml";
    public NetworkEventDateTimeController(ISessionService sessionService, IValidator<EventDateTimeViewModel> validator)
    {
        _sessionService = sessionService;
        _validator = validator;
    }

    [Authorize(Roles = Roles.ManageEventsRole)]
    [HttpGet]
    public IActionResult Get()
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();
        var model = GetViewModel(sessionModel);
        return View(ViewPath, model);
    }

    [HttpPost]
    public IActionResult Post(EventDateTimeViewModel submitModel)
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();
        sessionModel.DateOfEvent = submitModel.DateOfEvent;
        sessionModel.StartHour = submitModel.StartHour;
        sessionModel.StartMinutes = submitModel.StartMinutes;
        sessionModel.EndHour = submitModel.EndHour;
        sessionModel.EndMinutes = submitModel.EndMinutes;

        var result = _validator.Validate(submitModel);

        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            return View(ViewPath, GetViewModel(sessionModel));
        }

        _sessionService.Set(sessionModel);

        return RedirectToRoute(RouteNames.ManageEvent.EventDateTime);
    }

    private EventDateTimeViewModel GetViewModel(EventSessionModel sessionModel)
    {
        return new EventDateTimeViewModel
        {
            DateOfEvent = sessionModel.DateOfEvent,
            StartHour = sessionModel.StartHour,
            StartMinutes = sessionModel.StartMinutes,
            EndHour = sessionModel.EndHour,
            EndMinutes = sessionModel.EndMinutes,
            CancelLink = Url.RouteUrl(RouteNames.NetworkEvents)!,
            PostLink = Url.RouteUrl(RouteNames.ManageEvent.EventDateTime)!,
            PageTitle = Application.Constants.CreateEvent.PageTitle
        };
    }
}
