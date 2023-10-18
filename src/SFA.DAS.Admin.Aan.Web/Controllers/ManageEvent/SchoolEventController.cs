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
[Route("events/new/school")]
public class SchoolEventController : Controller
{
    private readonly ISessionService _sessionService;
    private readonly IValidator<IsAtSchoolViewModel> _eventAtSchoolValidator;
    private readonly IValidator<SchoolNameViewModel> _eventSchoolNameValidator;

    public const string EventAtSchoolViewPath = "~/Views/ManageEvent/IsAtSchool.cshtml";
    public const string EventSchoolNameViewPath = "~/Views/ManageEvent/SchoolName.cshtml";

    public SchoolEventController(ISessionService sessionService, IValidator<IsAtSchoolViewModel> eventAtSchoolValidator, IValidator<SchoolNameViewModel> eventSchoolNameValidator)
    {
        _sessionService = sessionService;
        _eventAtSchoolValidator = eventAtSchoolValidator;
        _eventSchoolNameValidator = eventSchoolNameValidator;
    }

    [HttpGet]
    [Route("question", Name = RouteNames.ManageEvent.IsAtSchool)]
    public IActionResult GetEventIsAtSchool()
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();
        var model = GetViewModelEventIsAtSchool(sessionModel);
        return View(EventAtSchoolViewPath, model);
    }

    [HttpPost]
    [Route("question", Name = RouteNames.ManageEvent.IsAtSchool)]
    public IActionResult PostIsAtSchool(IsAtSchoolViewModel submitModel)
    {
        var result = _eventAtSchoolValidator.Validate(submitModel);

        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            return View(EventAtSchoolViewPath, submitModel);
        }

        var sessionModel = _sessionService.Get<EventSessionModel>();

        sessionModel.IsAtSchool = submitModel.IsAtSchool;
        sessionModel.DirectCallFromCheckYourAnswers = false;
        _sessionService.Set(sessionModel);

        if (sessionModel.IsAtSchool == true) return RedirectToRoute(RouteNames.ManageEvent.SchoolName);

        if (sessionModel.HasSeenPreview)
        {
            return RedirectToRoute(RouteNames.ManageEvent.CheckYourAnswers);
        }

        return RedirectToRoute(RouteNames.ManageEvent.OrganiserDetails);
    }

    [HttpGet]
    [Route("name", Name = RouteNames.ManageEvent.SchoolName)]
    public IActionResult GetSchoolName()
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();
        var model = GetViewModelEventSchoolName(sessionModel);
        return View(EventSchoolNameViewPath, model);
    }

    [HttpPost]
    [Route("name", Name = RouteNames.ManageEvent.SchoolName)]
    public IActionResult PostSchoolName(SchoolNameViewModel submitModel)
    {
        var result = _eventSchoolNameValidator.Validate(submitModel);

        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            return View(EventSchoolNameViewPath, submitModel);
        }

        var sessionModel = _sessionService.Get<EventSessionModel>();

        sessionModel.SchoolName = submitModel.Name;
        sessionModel.Urn = submitModel.Urn;
        _sessionService.Set(sessionModel);

        if (sessionModel.HasSeenPreview)
        {
            return RedirectToRoute(RouteNames.ManageEvent.CheckYourAnswers);
        }

        return RedirectToRoute(RouteNames.ManageEvent.OrganiserDetails);
    }

    private IsAtSchoolViewModel GetViewModelEventIsAtSchool(EventSessionModel sessionModel)
    {
        var cancelLink = Url.RouteUrl(RouteNames.NetworkEvents)!;

        if (sessionModel.HasSeenPreview)
        {
            cancelLink = Url.RouteUrl(RouteNames.ManageEvent.CheckYourAnswers)!;
        }

        return new IsAtSchoolViewModel
        {
            IsAtSchool = sessionModel?.IsAtSchool,
            CancelLink = cancelLink,
            PostLink = Url.RouteUrl(RouteNames.ManageEvent.IsAtSchool)!,
            PageTitle = Application.Constants.CreateEvent.PageTitle
        };
    }

    private SchoolNameViewModel GetViewModelEventSchoolName(EventSessionModel sessionModel)
    {
        var searchResult = $"{sessionModel?.SchoolName} (URN: {sessionModel?.Urn})";

        var cancelLink = Url.RouteUrl(RouteNames.NetworkEvents)!;

        if (sessionModel!.HasSeenPreview)
        {
            cancelLink = Url.RouteUrl(RouteNames.ManageEvent.CheckYourAnswers)!;
        }

        return new SchoolNameViewModel
        {
            SearchResult = searchResult,
            CancelLink = cancelLink,
            PostLink = Url.RouteUrl(RouteNames.ManageEvent.SchoolName)!,
            PageTitle = Application.Constants.CreateEvent.PageTitle,
            DirectCallFromCheckYourAnswers = sessionModel.DirectCallFromCheckYourAnswers,
            HasSeenPreview = sessionModel.HasSeenPreview
        };
    }
}
