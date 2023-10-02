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
[Route("events/new/dateandtime", Name = RouteNames.ManageEvent.DateTime)]
public class DateAndTimeController : Controller
{
    private readonly ISessionService _sessionService;
    private readonly IValidator<DateAndTimeViewModel> _validator;

    public const string ViewPath = "~/Views/ManageEvent/DateAndTime.cshtml";
    public DateAndTimeController(ISessionService sessionService, IValidator<DateAndTimeViewModel> validator)
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
    public IActionResult Post(DateAndTimeViewModel submitModel)
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

        return RedirectToRoute(RouteNames.ManageEvent.Location);
    }

    private DateAndTimeViewModel GetViewModel(EventSessionModel sessionModel)
    {
        return new DateAndTimeViewModel
        {
            DateOfEvent = sessionModel.DateOfEvent,
            StartHour = sessionModel.StartHour,
            StartMinutes = sessionModel.StartMinutes,
            EndHour = sessionModel.EndHour,
            EndMinutes = sessionModel.EndMinutes,
            CancelLink = Url.RouteUrl(RouteNames.NetworkEvents)!,
            PostLink = Url.RouteUrl(RouteNames.ManageEvent.DateTime)!,
            PageTitle = Application.Constants.CreateEvent.PageTitle
        };
    }
}
