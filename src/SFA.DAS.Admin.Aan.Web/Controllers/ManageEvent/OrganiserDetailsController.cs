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
public class OrganiserDetailsController : Controller
{

    private readonly ISessionService _sessionService;
    private readonly IValidator<OrganiserDetailsViewModel> _organiserNameValidator;

    public OrganiserDetailsController(ISessionService sessionService, IValidator<OrganiserDetailsViewModel> organiserNameValidator)
    {
        _sessionService = sessionService;
        _organiserNameValidator = organiserNameValidator;
    }

    public const string OrganiserDetailsViewPath = "~/Views/ManageEvent/OrganiserDetails.cshtml";


    [HttpGet]
    [Route("events/new/organiser", Name = RouteNames.CreateEvent.OrganiserDetails)]
    [Route("events/{calendarEventId}/organiser", Name = RouteNames.UpdateEvent.UpdateOrganiserDetails)]
    public IActionResult Get()
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();
        var augmentedModel = GetViewModel(sessionModel);

        return View(OrganiserDetailsViewPath, augmentedModel);
    }

    [HttpPost]
    [Route("events/new/organiser", Name = RouteNames.CreateEvent.OrganiserDetails)]
    [Route("events/{calendarEventId}/organiser", Name = RouteNames.UpdateEvent.UpdateOrganiserDetails)]
    public IActionResult Post(OrganiserDetailsViewModel submitModel)
    {
        var result = _organiserNameValidator.Validate(submitModel);

        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            return View(OrganiserDetailsViewPath, submitModel);
        }

        var sessionModel = _sessionService.Get<EventSessionModel>();
        sessionModel.ContactName = submitModel.OrganiserName;
        sessionModel.ContactEmail = submitModel.OrganiserEmail;

        if (sessionModel.IsAlreadyPublished)
        {
            sessionModel.HasChangedEvent = true;
        }

        _sessionService.Set(sessionModel);

        if (sessionModel.IsAlreadyPublished)
        {
            return RedirectToRoute(RouteNames.CalendarEvent, new { sessionModel.CalendarEventId });
        }

        if (sessionModel.HasSeenPreview)
        {
            return RedirectToRoute(RouteNames.CreateEvent.CheckYourAnswers);
        }

        return RedirectToRoute(RouteNames.CreateEvent.NumberOfAttendees);
    }

    private OrganiserDetailsViewModel GetViewModel(EventSessionModel sessionModel)
    {
        string cancelLink;
        string postLink;

        if (sessionModel.IsAlreadyPublished)
        {
            cancelLink = Url.RouteUrl(RouteNames.CalendarEvent, new { sessionModel.CalendarEventId })!;
            postLink = Url.RouteUrl(RouteNames.UpdateEvent.UpdateOrganiserDetails, new { sessionModel.CalendarEventId })!;
        }
        else
        {
            cancelLink = Url.RouteUrl(RouteNames.NetworkEvents)!;
            postLink = Url.RouteUrl(RouteNames.CreateEvent.OrganiserDetails)!;

            if (sessionModel.HasSeenPreview)
            {
                cancelLink = Url.RouteUrl(RouteNames.CreateEvent.CheckYourAnswers)!;
            }
        }

        return new OrganiserDetailsViewModel
        {
            OrganiserName = sessionModel.ContactName?.Trim(),
            OrganiserEmail = sessionModel.ContactEmail?.Trim(),
            CancelLink = cancelLink,
            PostLink = postLink,
            PageTitle = sessionModel.PageTitle
        };
    }
}