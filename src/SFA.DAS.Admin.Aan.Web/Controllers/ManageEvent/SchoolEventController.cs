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
    private readonly IValidator<EventSchoolNameViewModel> _eventSchoolNameValidator;

    public const string EventAtSchoolViewPath = "~/Views/ManageEvent/EventIsAtSchool.cshtml";
    public const string EventSchoolNameViewPath = "~/Views/ManageEvent/EventSchoolName.cshtml";

    public SchoolEventController(ISessionService sessionService, IValidator<EventAtSchoolViewModel> eventAtSchoolValidator, IValidator<EventSchoolNameViewModel> eventSchoolNameValidator)
    {
        _sessionService = sessionService;
        _eventAtSchoolValidator = eventAtSchoolValidator;
        _eventSchoolNameValidator = eventSchoolNameValidator;
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

        if (sessionModel.IsAtSchool == true) return RedirectToRoute(RouteNames.ManageEvent.EventSchoolName);
        return RedirectToRoute(RouteNames.ManageEvent.EventIsAtSchool);
    }

    [HttpGet]
    [Route("name", Name = RouteNames.ManageEvent.EventSchoolName)]
    public IActionResult GetSchoolName()
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();
        var model = GetViewModelEventSchoolName(sessionModel);
        return View(EventSchoolNameViewPath, model);
    }

    [HttpPost]
    [Route("name", Name = RouteNames.ManageEvent.EventSchoolName)]
    public IActionResult PostSchoolName(EventSchoolNameViewModel submitModel)
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();

        sessionModel.SchoolName = submitModel.Name;
        sessionModel.Urn = submitModel.Urn;

        var result = _eventSchoolNameValidator.Validate(submitModel);

        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            return View(EventSchoolNameViewPath, GetViewModelEventSchoolName(sessionModel));
        }

        _sessionService.Set(sessionModel);

        return RedirectToRoute(RouteNames.ManageEvent.EventSchoolName);
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

    private EventSchoolNameViewModel GetViewModelEventSchoolName(EventSessionModel sessionModel)
    {
        var searchResult = $"{sessionModel?.SchoolName} (URN: {sessionModel.Urn})";

        return new EventSchoolNameViewModel
        {
            SearchResult = searchResult,
            CancelLink = Url.RouteUrl(RouteNames.NetworkEvents)!,
            PostLink = Url.RouteUrl(RouteNames.ManageEvent.EventIsAtSchool)!,
            PageTitle = Application.Constants.CreateEvent.PageTitle
        };
    }
}
