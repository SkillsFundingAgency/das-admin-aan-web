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
[Route("events/new/school")]
public class SchoolEventController : Controller
{
    private readonly ISessionService _sessionService;
    private readonly IValidator<EventAtSchoolViewModel> _eventAtSchoolValidator;

    public const string EventAtSchoolViewPath = "~/Views/ManageEvent/EventIsAtSchool.cshtml";

    public SchoolEventController(ISessionService sessionService, IValidator<EventAtSchoolViewModel> eventAtSchoolValidator)
    {
        _sessionService = sessionService;
        _eventAtSchoolValidator = eventAtSchoolValidator;
    }

    [HttpGet]
    [Route("question", Name = RouteNames.ManageEvent.EventIsAtSchool)]
    public IActionResult GetEventIsAtSchool()
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();
        var model = GetViewModelEventIsAtSchool(sessionModel);
        return View(EventAtSchoolViewPath, model);
    }

    [HttpPost]
    [Route("question", Name = RouteNames.ManageEvent.EventIsAtSchool)]
    public IActionResult PostHasGuestSpeakers(EventAtSchoolViewModel submitModel)
    {
        var result = _eventAtSchoolValidator.Validate(submitModel);

        var sessionModel = _sessionService.Get<EventSessionModel>();

        if (!result.IsValid)
        {

            result.AddToModelState(ModelState);
            return View(EventAtSchoolViewPath, GetViewModelEventIsAtSchool(sessionModel));
        }

        sessionModel.IsAtSchool = submitModel.IsAtSchool;
        _sessionService.Set(sessionModel);

        if (sessionModel.IsAtSchool == true) return RedirectToRoute(RouteNames.ManageEvent.EventIsAtSchool);
        return RedirectToRoute(RouteNames.ManageEvent.EventIsAtSchool);
    }

    private EventAtSchoolViewModel GetViewModelEventIsAtSchool(EventSessionModel sessionModel)
    {
        return new EventAtSchoolViewModel
        {
            IsAtSchool = sessionModel?.IsAtSchool,
            CancelLink = Url.RouteUrl(RouteNames.NetworkEvents)!,
            PostLink = Url.RouteUrl(RouteNames.ManageEvent.EventIsAtSchool)!,
            PageTitle = Application.Constants.CreateEvent.PageTitle
        };
    }
}
