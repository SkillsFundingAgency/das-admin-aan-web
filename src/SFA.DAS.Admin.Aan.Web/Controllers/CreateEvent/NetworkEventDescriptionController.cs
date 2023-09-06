﻿using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

namespace SFA.DAS.Admin.Aan.Web.Controllers.CreateEvent;

[Authorize]
[Route("manage-events/new/event-description", Name = RouteNames.CreateEvent.EventDescription)]
public class NetworkEventDescriptionController : Controller
{
    private readonly ISessionService _sessionService;
    private readonly IValidator<CreateEventDescriptionViewModel> _validator;

    public const string ViewPath = "~/Views/NetworkEvent/EventDescription.cshtml";
    public NetworkEventDescriptionController(ISessionService sessionService, IValidator<CreateEventDescriptionViewModel> validator)
    {
        _sessionService = sessionService;
        _validator = validator;
    }
    [HttpGet]
    public IActionResult Get()
    {
        var sessionModel = _sessionService.Get<CreateEventSessionModel>();
        var model = GetViewModel(sessionModel);
        return View(ViewPath, model);
    }

    [HttpPost]
    public IActionResult Post(CreateEventDescriptionViewModel submitModel)
    {
        var sessionModel = _sessionService.Get<CreateEventSessionModel?>();
        if (sessionModel == null) return RedirectToAction("Get", "NetworkEventFormat");

        sessionModel.EventOutline = submitModel.EventOutline;
        sessionModel.EventSummary = submitModel.EventSummary;
        sessionModel.GuestSpeaker = submitModel.GuestSpeaker;

        var result = _validator.Validate(submitModel);

        if (!result.IsValid)
        {

            result.AddToModelState(ModelState);
            return View(ViewPath, GetViewModel(sessionModel));
        }

        _sessionService.Set(sessionModel);
        return RedirectToAction("Get", "NetworkEventDescription");
    }

    private CreateEventDescriptionViewModel GetViewModel(CreateEventSessionModel sessionModel)
    {
        return new CreateEventDescriptionViewModel
        {
            EventOutline = sessionModel?.EventOutline,
            EventSummary = sessionModel?.EventSummary,
            GuestSpeaker = sessionModel?.GuestSpeaker,
            BackLink = Url.RouteUrl(RouteNames.NetworkEvents)!
        };
    }
}
