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
[Route("events/new/organiser", Name = RouteNames.ManageEvent.EventOrganiserName)]
public class NetworkEventOrganiserNameController : Controller
{

    private readonly ISessionService _sessionService;
    private readonly IValidator<EventOrganiserNameViewModel> _organiserNameValidator;

    public NetworkEventOrganiserNameController(ISessionService sessionService, IValidator<EventOrganiserNameViewModel> organiserNameValidator)
    {
        _sessionService = sessionService;
        _organiserNameValidator = organiserNameValidator;
    }

    public const string OrganiserNameViewPath = "~/Views/ManageEvent/EventOrganiserName.cshtml";


    [HttpGet]
    public IActionResult Get()
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();
        var augmentedModel = GetOrganiserNameViewModel(sessionModel);

        return View(OrganiserNameViewPath, augmentedModel);
    }

    [HttpPost]
    public IActionResult Post(EventOrganiserNameViewModel submitModel)
    {
        var result = _organiserNameValidator.Validate(submitModel);

        var sessionModel = _sessionService.Get<EventSessionModel>();
        sessionModel.OrganiserName = submitModel.OrganiserName;
        sessionModel.OrganiserEmail = submitModel.OrganiserEmail;

        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            return View(OrganiserNameViewPath, GetOrganiserNameViewModel(sessionModel));
        }

        _sessionService.Set(sessionModel);
        return RedirectToRoute(RouteNames.ManageEvent.EventOrganiserName);
    }

    private EventOrganiserNameViewModel GetOrganiserNameViewModel(EventSessionModel originalModel)
    {
        return new EventOrganiserNameViewModel
        {
            OrganiserName = originalModel.OrganiserName,
            OrganiserEmail = originalModel.OrganiserEmail,
            CancelLink = Url.RouteUrl(RouteNames.NetworkEvents)!,
            PostLink = Url.RouteUrl(RouteNames.ManageEvent.EventOrganiserName)!,
            PageTitle = Application.Constants.CreateEvent.PageTitle
        };
    }
}