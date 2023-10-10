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
[Route("events/new/organiser", Name = RouteNames.ManageEvent.OrganiserDetails)]
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
    public IActionResult Get()
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();
        var augmentedModel = GetOrganiserNameViewModel(sessionModel);

        return View(OrganiserDetailsViewPath, augmentedModel);
    }

    [HttpPost]
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

        _sessionService.Set(sessionModel);
        return RedirectToRoute(RouteNames.ManageEvent.NumberOfAttendees);
    }

    private OrganiserDetailsViewModel GetOrganiserNameViewModel(EventSessionModel sessionModel)
    {
        return new OrganiserDetailsViewModel
        {
            OrganiserName = sessionModel.ContactName?.Trim(),
            OrganiserEmail = sessionModel.ContactEmail?.Trim(),
            CancelLink = Url.RouteUrl(RouteNames.NetworkEvents)!,
            PostLink = Url.RouteUrl(RouteNames.ManageEvent.OrganiserDetails)!,
            PageTitle = Application.Constants.CreateEvent.PageTitle
        };
    }
}