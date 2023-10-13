using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Authentication;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.Controllers.ManageEvent;


[Authorize(Roles = Roles.ManageEventsRole)]
[Route("events/new/dateandtime", Name = RouteNames.ManageEvent.DateAndTime)]
public class EventDateAndTimeController : Controller
{
    private readonly ISessionService _sessionService;
    private readonly IValidator<EventDateAndTimeViewModel> _validator;

    public const string ViewPath = "~/Views/ManageEvent/EventDateAndTime.cshtml";
    public EventDateAndTimeController(ISessionService sessionService, IValidator<EventDateAndTimeViewModel> validator)
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
    public IActionResult Post(EventDateAndTimeViewModel submitModel)
    {
        var result = _validator.Validate(submitModel);

        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            return View(ViewPath, submitModel);
        }

        var sessionModel = _sessionService.Get<EventSessionModel>();
        sessionModel.DateOfEvent = submitModel.DateOfEvent;
        sessionModel.StartHour = submitModel.StartHour;
        sessionModel.StartMinutes = submitModel.StartMinutes;
        sessionModel.EndHour = submitModel.EndHour;
        sessionModel.EndMinutes = submitModel.EndMinutes;
        _sessionService.Set(sessionModel);

        if (sessionModel.HasSeenPreview)
        {
            return RedirectToRoute(RouteNames.ManageEvent.CheckYourAnswers);
        }

        return RedirectToRoute(RouteNames.ManageEvent.Location);
    }

    private EventDateAndTimeViewModel GetViewModel(EventSessionModel sessionModel)
    {
        var cancelLink = Url.RouteUrl(RouteNames.NetworkEvents)!;

        if (sessionModel.HasSeenPreview)
        {
            cancelLink = Url.RouteUrl(RouteNames.ManageEvent.CheckYourAnswers)!;
        }
        return new EventDateAndTimeViewModel
        {
            DateOfEvent = sessionModel.DateOfEvent,
            StartHour = sessionModel.StartHour,
            StartMinutes = sessionModel.StartMinutes,
            EndHour = sessionModel.EndHour,
            EndMinutes = sessionModel.EndMinutes,
            CancelLink = cancelLink,
            PostLink = Url.RouteUrl(RouteNames.ManageEvent.DateAndTime)!,
            PageTitle = Application.Constants.CreateEvent.PageTitle
        };
    }
}
