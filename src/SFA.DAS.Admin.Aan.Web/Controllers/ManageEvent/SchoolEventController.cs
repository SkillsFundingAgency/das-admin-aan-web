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
    [Route("events/new/school/question", Name = RouteNames.CreateEvent.IsAtSchool)]
    [Route("events/{calendarEventId}/school/question", Name = RouteNames.UpdateEvent.UpdateIsAtSchool)]
    public IActionResult GetEventIsAtSchool()
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();
        var model = GetViewModelEventIsAtSchool(sessionModel);
        return View(EventAtSchoolViewPath, model);
    }

    [HttpPost]
    [Route("events/new/school/question", Name = RouteNames.CreateEvent.IsAtSchool)]
    [Route("events/{calendarEventId}/school/question", Name = RouteNames.UpdateEvent.UpdateIsAtSchool)]
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
        sessionModel.IsDirectCallFromCheckYourAnswers = false;
        _sessionService.Set(sessionModel);

        if (sessionModel.IsAtSchool == true)
        {
            if (sessionModel.IsAlreadyPublished)
            {
                return RedirectToRoute(RouteNames.UpdateEvent.UpdateSchoolName, new { sessionModel.CalendarEventId });
            }

            return RedirectToRoute(RouteNames.CreateEvent.SchoolName);
        }

        if (sessionModel.IsAlreadyPublished)
        {
            return RedirectToRoute(RouteNames.CalendarEvent, new { sessionModel.CalendarEventId });
        }

        if (sessionModel.HasSeenPreview)
        {
            return RedirectToRoute(RouteNames.CreateEvent.CheckYourAnswers);
        }

        return RedirectToRoute(RouteNames.CreateEvent.OrganiserDetails);
    }

    [HttpGet]
    [Route("events/new/school/name", Name = RouteNames.CreateEvent.SchoolName)]
    [Route("events/{calendarEventId}/school/name", Name = RouteNames.UpdateEvent.UpdateSchoolName)]
    public IActionResult GetSchoolName()
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();
        var model = GetViewModelEventSchoolName(sessionModel);
        return View(EventSchoolNameViewPath, model);
    }

    [HttpPost]
    [Route("events/new/school/name", Name = RouteNames.CreateEvent.SchoolName)]
    [Route("events/{calendarEventId}/school/name", Name = RouteNames.UpdateEvent.UpdateSchoolName)]
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

        if (sessionModel.IsAlreadyPublished)
        {
            return RedirectToRoute(RouteNames.CalendarEvent, new { sessionModel.CalendarEventId });
        }

        if (sessionModel.HasSeenPreview)
        {
            return RedirectToRoute(RouteNames.CreateEvent.CheckYourAnswers);
        }

        return RedirectToRoute(RouteNames.CreateEvent.OrganiserDetails);
    }

    private IsAtSchoolViewModel GetViewModelEventIsAtSchool(EventSessionModel sessionModel)
    {
        var cancelLink = Url.RouteUrl(RouteNames.NetworkEvents)!;
        var postLink = Url.RouteUrl(RouteNames.CreateEvent.IsAtSchool)!;

        if (sessionModel.IsAlreadyPublished)
        {
            cancelLink = Url.RouteUrl(RouteNames.CalendarEvent, new { sessionModel.CalendarEventId });
            postLink = Url.RouteUrl(RouteNames.UpdateEvent.UpdateSchoolName, new { sessionModel.CalendarEventId });
        }
        else
        {
            if (sessionModel.HasSeenPreview)
            {
                cancelLink = Url.RouteUrl(RouteNames.CreateEvent.CheckYourAnswers)!;
            }
        }

        return new IsAtSchoolViewModel
        {
            IsAtSchool = sessionModel!.IsAtSchool,
            CancelLink = cancelLink,
            PostLink = postLink,
            PageTitle = sessionModel!.PageTitle
        };
    }

    private SchoolNameViewModel GetViewModelEventSchoolName(EventSessionModel sessionModel)
    {
        var searchResult = $"{sessionModel!.SchoolName} (URN: {sessionModel!.Urn})";

        var cancelLink = Url.RouteUrl(RouteNames.NetworkEvents)!;
        var postLink = Url.RouteUrl(RouteNames.CreateEvent.SchoolName)!;

        if (sessionModel!.IsAlreadyPublished)
        {
            cancelLink = Url.RouteUrl(RouteNames.CalendarEvent, new { sessionModel.CalendarEventId });
            postLink = Url.RouteUrl(RouteNames.UpdateEvent.UpdateSchoolName, new { sessionModel.CalendarEventId });
        }
        else
        {
            if (sessionModel!.HasSeenPreview)
            {
                cancelLink = Url.RouteUrl(RouteNames.CreateEvent.CheckYourAnswers)!;
            }
        }

        return new SchoolNameViewModel
        {
            SearchResult = searchResult,
            CancelLink = cancelLink,
            PostLink = postLink,
            PageTitle = sessionModel.PageTitle,
            DirectCallFromCheckYourAnswers = sessionModel.IsDirectCallFromCheckYourAnswers,
            HasSeenPreview = sessionModel.HasSeenPreview
        };
    }
}
