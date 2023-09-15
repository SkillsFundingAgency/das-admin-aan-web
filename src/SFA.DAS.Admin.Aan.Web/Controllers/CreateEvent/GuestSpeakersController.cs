using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Authentication;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

namespace SFA.DAS.Admin.Aan.Web.Controllers.CreateEvent;
[Authorize(Roles = Roles.ManageEventsRole)]
[Route("events/new/guestspeakers")]
public class GuestSpeakersController : Controller
{
    private readonly ISessionService _sessionService;
    private readonly IValidator<GuestSpeakerAddViewModel> _addGuestSpeakerValidator;
    private readonly IValidator<CreateEventHasGuestSpeakersViewModel> _hasGuestSpeakersValidator;

    public const string GuestSpeakerListViewPath = "~/Views/NetworkEvent/GuestSpeakerList.cshtml";
    public const string HasGuestSpeakersViewPath = "~/Views/NetworkEvent/EventGuestSpeaker.cshtml";
    public const string GuestSpeakerAddViewPath = "~/Views/NetworkEvent/GuestSpeakerAdd.cshtml";
    public GuestSpeakersController(ISessionService sessionService, IValidator<GuestSpeakerAddViewModel> addGuestSpeakerValidator, IValidator<CreateEventHasGuestSpeakersViewModel> hasGuestSpeakersValidator)
    {
        _sessionService = sessionService;
        _addGuestSpeakerValidator = addGuestSpeakerValidator;
        _hasGuestSpeakersValidator = hasGuestSpeakersValidator;
    }
    [HttpGet]
    [Route("question", Name = RouteNames.CreateEvent.EventHasGuestSpeakers)]
    public IActionResult GetHasGuestSpeakers()
    {
        var sessionModel = _sessionService.Get<CreateEventSessionModel>();
        var model = GetViewModelHasGuestSpeakers(sessionModel);
        return View(HasGuestSpeakersViewPath, model);
    }

    [HttpPost]
    [Route("question", Name = RouteNames.CreateEvent.EventHasGuestSpeakers)]
    public IActionResult PostHasGuestSpeakers(CreateEventHasGuestSpeakersViewModel submitModel)
    {
        var sessionModel = _sessionService.Get<CreateEventSessionModel?>();
        sessionModel!.HasGuestSpeakers = submitModel.HasGuestSpeakers;

        var result = _hasGuestSpeakersValidator.Validate(submitModel);

        if (!result.IsValid)
        {

            result.AddToModelState(ModelState);
            return View(HasGuestSpeakersViewPath, GetViewModelHasGuestSpeakers(sessionModel));
        }

        _sessionService.Set(sessionModel);

        if (sessionModel.HasGuestSpeakers == true) return RedirectToRoute(RouteNames.CreateEvent.GuestSpeakerList);
        return RedirectToRoute(RouteNames.CreateEvent.EventHasGuestSpeakers);
    }

    [HttpGet]
    [Route("add", Name = RouteNames.CreateEvent.GuestSpeakerAdd)]
    public IActionResult GetAddGuestSpeaker(GuestSpeakerAddViewModel model)
    {
        var augmentedModel = GetGuestSpeakerAddViewModel(model);

        return View(GuestSpeakerAddViewPath, augmentedModel);
    }


    [HttpGet]
    [Route("delete", Name = RouteNames.CreateEvent.GuestSpeakerDelete)]
    public IActionResult DeleteGuestSpeaker(int id)
    {
        var sessionModel = _sessionService.Get<CreateEventSessionModel?>();
        var currentGuestList = sessionModel!.GuestSpeakers;
        if (currentGuestList.Any())
        {
            var removeItem = currentGuestList.First(x => x.Id == id);
            currentGuestList.Remove(removeItem);
        }

        sessionModel.GuestSpeakers = currentGuestList;
        _sessionService.Set(sessionModel);
        return RedirectToRoute(RouteNames.CreateEvent.GuestSpeakerList);
    }

    [HttpPost]
    [Route("add", Name = RouteNames.CreateEvent.GuestSpeakerAdd)]
    public IActionResult PostAddGuestSpeaker(GuestSpeakerAddViewModel submitModel)
    {
        var result = _addGuestSpeakerValidator.Validate(submitModel);

        if (!result.IsValid)
        {

            result.AddToModelState(ModelState);
            return View(GuestSpeakerAddViewPath, GetGuestSpeakerAddViewModel(submitModel));
        }

        var sessionModel = _sessionService.Get<CreateEventSessionModel?>();
        var currentGuestList = sessionModel!.GuestSpeakers;

        var id = currentGuestList.Any() ? currentGuestList.Max(x => x.Id) + 1 : 1;


        currentGuestList.Add(new GuestSpeaker(submitModel.Name!, submitModel.JobRoleAndOrganisation!, id));
        sessionModel.GuestSpeakers = currentGuestList;

        _sessionService.Set(sessionModel);
        return RedirectToRoute(RouteNames.CreateEvent.GuestSpeakerList);
    }

    [HttpGet]
    [Route("", Name = RouteNames.CreateEvent.GuestSpeakerList)]
    public IActionResult GetSpeakerList()
    {
        var sessionModel = _sessionService.Get<CreateEventSessionModel>();
        var model = GetGuestSpeakerListViewModel(sessionModel);
        return View(GuestSpeakerListViewPath, model);
    }

    [HttpPost]
    [Route("", Name = RouteNames.CreateEvent.GuestSpeakerList)]
    public IActionResult PostGuestSpeakerList()
    {
        return RedirectToRoute(RouteNames.CreateEvent.GuestSpeakerList);
    }

    private GuestSpeakerAddViewModel GetGuestSpeakerAddViewModel(GuestSpeakerAddViewModel originalModel)
    {
        return new GuestSpeakerAddViewModel
        {
            Name = originalModel.Name,
            JobRoleAndOrganisation = originalModel.JobRoleAndOrganisation,
            CancelLink = Url.RouteUrl(RouteNames.CreateEvent.GuestSpeakerList)!,
            PostLink = Url.RouteUrl(RouteNames.CreateEvent.GuestSpeakerAdd)!,
            PageTitle = Application.Constants.CreateEvent.PageTitle
        };
    }


    private CreateEventHasGuestSpeakersViewModel GetViewModelHasGuestSpeakers(CreateEventSessionModel sessionModel)
    {
        return new CreateEventHasGuestSpeakersViewModel
        {
            HasGuestSpeakers = sessionModel?.HasGuestSpeakers,
            CancelLink = Url.RouteUrl(RouteNames.NetworkEvents)!,
            PostLink = Url.RouteUrl(RouteNames.CreateEvent.EventHasGuestSpeakers)!,
            PageTitle = Application.Constants.CreateEvent.PageTitle
        };
    }

    private CreateEventGuestSpeakerListViewModel GetGuestSpeakerListViewModel(CreateEventSessionModel sessionModel)
    {
        return new CreateEventGuestSpeakerListViewModel
        {
            GuestSpeakers = sessionModel.GuestSpeakers,
            CancelLink = Url.RouteUrl(RouteNames.NetworkEvents)!,
            PostLink = Url.RouteUrl(RouteNames.CreateEvent.GuestSpeakerList)!,
            AddGuestSpeakerLink = Url.RouteUrl(RouteNames.CreateEvent.GuestSpeakerAdd)!,
            DeleteSpeakerLink = Url.RouteUrl(RouteNames.CreateEvent.GuestSpeakerDelete)!,
            PageTitle = Application.Constants.CreateEvent.PageTitle
        };
    }
}