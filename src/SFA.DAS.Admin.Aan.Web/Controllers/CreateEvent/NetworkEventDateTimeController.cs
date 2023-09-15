using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Authentication;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

namespace SFA.DAS.Admin.Aan.Web.Controllers.CreateEvent;

[Route("events/new/datetime", Name = RouteNames.CreateEvent.EventDateTime)]
public class NetworkEventDateTimeController : Controller
{
    private readonly ISessionService _sessionService;
    private readonly IValidator<CreateEventDateTimeViewModel> _validator;

    public const string ViewPath = "~/Views/NetworkEvent/EventDateTime.cshtml";
    public NetworkEventDateTimeController(ISessionService sessionService, IValidator<CreateEventDateTimeViewModel> validator)
    {
        _sessionService = sessionService;
        _validator = validator;
    }

    [Authorize(Roles = Roles.ManageEventsRole)]
    [HttpGet]
    public IActionResult Get()
    {
        var sessionModel = _sessionService.Get<CreateEventSessionModel>();
        var model = GetViewModel(sessionModel);
        return View(ViewPath, model);
    }


    private CreateEventDateTimeViewModel GetViewModel(CreateEventSessionModel sessionModel)
    {
        return new CreateEventDateTimeViewModel
        {
            DateOfEvent = sessionModel.DateOfEvent,
            StartHour = sessionModel.StartHour,
            StartMinutes = sessionModel.StartMinutes,
            EndHour = sessionModel.EndHour,
            EndMinutes = sessionModel.EndMinutes,
            CancelLink = Url.RouteUrl(RouteNames.NetworkEvents)!,
            PostLink = Url.RouteUrl(RouteNames.CreateEvent.EventDateTime)!,
            PageTitle = Application.Constants.CreateEvent.PageTitle
        };
    }
}
