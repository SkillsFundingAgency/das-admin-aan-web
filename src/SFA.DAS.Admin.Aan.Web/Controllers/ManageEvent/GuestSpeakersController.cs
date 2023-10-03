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
[Route("events/new/guestspeakers")]
public class GuestSpeakersController : Controller
{
    private readonly ISessionService _sessionService;
    private readonly IValidator<GuestSpeakerAddViewModel> _addGuestSpeakerValidator;
    private readonly IValidator<HasGuestSpeakersViewModel> _hasGuestSpeakersValidator;

    public const string GuestSpeakerListViewPath = "~/Views/ManageEvent/GuestSpeakerList.cshtml";
    public const string HasGuestSpeakersViewPath = "~/Views/ManageEvent/HasGuestSpeakers.cshtml";
    public const string GuestSpeakerAddViewPath = "~/Views/ManageEvent/GuestSpeakerAdd.cshtml";
    public GuestSpeakersController(ISessionService sessionService, IValidator<GuestSpeakerAddViewModel> addGuestSpeakerValidator, IValidator<HasGuestSpeakersViewModel> hasGuestSpeakersValidator)
    {
        _sessionService = sessionService;
        _addGuestSpeakerValidator = addGuestSpeakerValidator;
        _hasGuestSpeakersValidator = hasGuestSpeakersValidator;
    }
    [HttpGet]
    [Route("question", Name = RouteNames.ManageEvent.HasGuestSpeakers)]
    public IActionResult GetHasGuestSpeakers()
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();
        var model = GetViewModelHasGuestSpeakers(sessionModel);
        return View(HasGuestSpeakersViewPath, model);
    }

    [HttpPost]
    [Route("question", Name = RouteNames.ManageEvent.HasGuestSpeakers)]
    public IActionResult PostHasGuestSpeakers(HasGuestSpeakersViewModel submitModel)
    {
        var result = _hasGuestSpeakersValidator.Validate(submitModel);

        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            return View(HasGuestSpeakersViewPath, submitModel);
        }

        var sessionModel = _sessionService.Get<EventSessionModel>();
        sessionModel.HasGuestSpeakers = submitModel.HasGuestSpeakers;
        _sessionService.Set(sessionModel);

        if (sessionModel.HasGuestSpeakers == true) return RedirectToRoute(RouteNames.ManageEvent.GuestSpeakerList);
        return RedirectToRoute(RouteNames.ManageEvent.DateAndTime);
    }

    [HttpGet]
    [Route("add", Name = RouteNames.ManageEvent.GuestSpeakerAdd)]
    public IActionResult GetAddGuestSpeaker()
    {
        var augmentedModel = new GuestSpeakerAddViewModel
        {
            CancelLink = Url.RouteUrl(RouteNames.ManageEvent.GuestSpeakerList)!,
            PostLink = Url.RouteUrl(RouteNames.ManageEvent.GuestSpeakerAdd)!,
            PageTitle = Application.Constants.CreateEvent.PageTitle
        };

        return View(GuestSpeakerAddViewPath, augmentedModel);
    }

    [HttpPost]
    [Route("add", Name = RouteNames.ManageEvent.GuestSpeakerAdd)]
    public IActionResult PostAddGuestSpeaker(GuestSpeakerAddViewModel submitModel)
    {
        var result = _addGuestSpeakerValidator.Validate(submitModel);

        if (!result.IsValid)
        {

            result.AddToModelState(ModelState);
            return View(GuestSpeakerAddViewPath, submitModel);
        }

        var sessionModel = _sessionService.Get<EventSessionModel>();
        var currentGuestList = sessionModel.GuestSpeakers;

        var id = currentGuestList.Any() ? currentGuestList.Max(x => x.Id) + 1 : 1;

        currentGuestList.Add(new GuestSpeaker(submitModel.Name!, submitModel.JobRoleAndOrganisation!, id));
        sessionModel.GuestSpeakers = currentGuestList;

        _sessionService.Set(sessionModel);
        return RedirectToRoute(RouteNames.ManageEvent.GuestSpeakerList);
    }

    [HttpGet]
    [Route("delete", Name = RouteNames.ManageEvent.GuestSpeakerDelete)]
    public IActionResult DeleteGuestSpeaker(int id)
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();
        var currentGuestList = sessionModel!.GuestSpeakers;
        if (currentGuestList.Any())
        {
            var removeItem = currentGuestList.First(x => x.Id == id);
            currentGuestList.Remove(removeItem);
        }

        sessionModel.GuestSpeakers = currentGuestList;
        _sessionService.Set(sessionModel);
        return RedirectToRoute(RouteNames.ManageEvent.GuestSpeakerList);
    }

    [HttpGet]
    [Route("", Name = RouteNames.ManageEvent.GuestSpeakerList)]
    public IActionResult GetSpeakerList()
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();
        var model = GetGuestSpeakerListViewModel(sessionModel);
        return View(GuestSpeakerListViewPath, model);
    }

    [HttpPost]
    [Route("", Name = RouteNames.ManageEvent.GuestSpeakerList)]
    public IActionResult PostGuestSpeakerList()
    {
        return RedirectToRoute(RouteNames.ManageEvent.DateAndTime);
    }

    private HasGuestSpeakersViewModel GetViewModelHasGuestSpeakers(EventSessionModel sessionModel)
    {
        return new HasGuestSpeakersViewModel
        {
            HasGuestSpeakers = sessionModel?.HasGuestSpeakers,
            CancelLink = Url.RouteUrl(RouteNames.NetworkEvents)!,
            PostLink = Url.RouteUrl(RouteNames.ManageEvent.HasGuestSpeakers)!,
            PageTitle = Application.Constants.CreateEvent.PageTitle
        };
    }

    private GuestSpeakerListViewModel GetGuestSpeakerListViewModel(EventSessionModel sessionModel)
    {
        return new GuestSpeakerListViewModel
        {
            GuestSpeakers = sessionModel.GuestSpeakers,
            CancelLink = Url.RouteUrl(RouteNames.NetworkEvents)!,
            PostLink = Url.RouteUrl(RouteNames.ManageEvent.GuestSpeakerList)!,
            AddGuestSpeakerLink = Url.RouteUrl(RouteNames.ManageEvent.GuestSpeakerAdd)!,
            DeleteSpeakerLink = Url.RouteUrl(RouteNames.ManageEvent.GuestSpeakerDelete)!,
            PageTitle = Application.Constants.CreateEvent.PageTitle
        };
    }
}