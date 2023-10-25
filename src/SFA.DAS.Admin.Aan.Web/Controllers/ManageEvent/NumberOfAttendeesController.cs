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
[Route("events/new/attendees", Name = RouteNames.ManageEvent.NumberOfAttendees)]
public class NumberOfAttendeesController : Controller
{
    private readonly ISessionService _sessionService;
    private readonly IValidator<NumberOfAttendeesViewModel> _validator;

    public const string ViewPath = "~/Views/ManageEvent/NumberOfAttendees.cshtml";
    public NumberOfAttendeesController(ISessionService sessionService, IValidator<NumberOfAttendeesViewModel> validator)
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
    public IActionResult Post(NumberOfAttendeesViewModel submitModel)
    {
        var result = _validator.Validate(submitModel);


        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            return View(ViewPath, submitModel);
        }

        var sessionModel = _sessionService.Get<EventSessionModel>();
        sessionModel.PlannedAttendees = submitModel.NumberOfAttendees;
        _sessionService.Set(sessionModel);

        return RedirectToRoute(RouteNames.ManageEvent.CheckYourAnswers);
    }

    private NumberOfAttendeesViewModel GetViewModel(EventSessionModel sessionModel)
    {
        var cancelLink = Url.RouteUrl(RouteNames.NetworkEvents)!;

        if (sessionModel.HasSeenPreview)
        {
            cancelLink = Url.RouteUrl(RouteNames.ManageEvent.CheckYourAnswers)!;
        }

        return new NumberOfAttendeesViewModel
        {
            NumberOfAttendees = sessionModel.PlannedAttendees,
            CancelLink = cancelLink,
            PostLink = Url.RouteUrl(RouteNames.ManageEvent.NumberOfAttendees)!,
            PageTitle = Application.Constants.CreateEvent.PageTitle
        };
    }
}
