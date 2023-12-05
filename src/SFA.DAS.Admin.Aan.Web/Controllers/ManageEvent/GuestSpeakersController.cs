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
    [Route("events/new/guestspeakers/question", Name = RouteNames.CreateEvent.HasGuestSpeakers)]
    [Route("events/{calendarEventId}/guestspeakers/question", Name = RouteNames.UpdateEvent.UpdateHasGuestSpeakers)]
    public IActionResult GetHasGuestSpeakers()
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();
        var model = GetViewModelHasGuestSpeakers(sessionModel);
        return View(HasGuestSpeakersViewPath, model);
    }

    [HttpPost]
    [Route("events/new/guestspeakers/question", Name = RouteNames.CreateEvent.HasGuestSpeakers)]
    [Route("events/{calendarEventId}/guestspeakers/question", Name = RouteNames.UpdateEvent.UpdateHasGuestSpeakers)]

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
        if (sessionModel.HasGuestSpeakers == false)
        {
            sessionModel.GuestSpeakers = new List<GuestSpeaker>();
        }

        sessionModel.IsDirectCallFromCheckYourAnswers = false;

        if (sessionModel.IsAlreadyPublished)
        {
            sessionModel.HasChangedEvent = true;
        }

        _sessionService.Set(sessionModel);

        if (sessionModel.IsAlreadyPublished)
        {
            if (sessionModel.HasGuestSpeakers == true)
            {
                return RedirectToRoute(RouteNames.UpdateEvent.UpdateGuestSpeakerList, new { sessionModel.CalendarEventId });
            }

            return RedirectToRoute(RouteNames.CalendarEvent, new { sessionModel.CalendarEventId });
        }


        if (sessionModel.HasGuestSpeakers == true) return RedirectToRoute(RouteNames.CreateEvent.GuestSpeakerList);

        if (sessionModel.HasSeenPreview)
        {
            return RedirectToRoute(RouteNames.CreateEvent.CheckYourAnswers);
        }

        return RedirectToRoute(RouteNames.CreateEvent.DateAndTime);
    }

    [HttpGet]
    [Route("events/new/guestspeakers/add", Name = RouteNames.CreateEvent.GuestSpeakerAdd)]
    [Route("events/{calendarEventId}/guestspeakers/add", Name = RouteNames.UpdateEvent.UpdateGuestSpeakerAdd)]
    public IActionResult GetAddGuestSpeaker()
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();

        var cancelLink = Url.RouteUrl(RouteNames.CreateEvent.GuestSpeakerList)!;
        var postLink = Url.RouteUrl(RouteNames.CreateEvent.GuestSpeakerAdd)!;

        if (sessionModel.IsAlreadyPublished)
        {
            cancelLink = Url.RouteUrl(RouteNames.UpdateEvent.UpdateGuestSpeakerList, new { sessionModel.CalendarEventId })!;
            postLink = Url.RouteUrl(RouteNames.UpdateEvent.UpdateGuestSpeakerAdd, new { sessionModel.CalendarEventId })!;
        }

        var augmentedModel = new GuestSpeakerAddViewModel
        {
            CancelLink = cancelLink,
            PostLink = postLink,
            PageTitle = sessionModel.PageTitle
        };

        return View(GuestSpeakerAddViewPath, augmentedModel);
    }

    [HttpPost]
    [Route("events/new/guestspeakers/add", Name = RouteNames.CreateEvent.GuestSpeakerAdd)]
    [Route("events/{calendarEventId}/guestspeakers/add", Name = RouteNames.UpdateEvent.UpdateGuestSpeakerAdd)]
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

        currentGuestList.Add(new GuestSpeaker(submitModel.Name!.Trim(), submitModel.JobRoleAndOrganisation!.Trim(), id));
        sessionModel.GuestSpeakers = currentGuestList;

        if (sessionModel.IsAlreadyPublished)
        {
            sessionModel.HasChangedEvent = true;
        }

        _sessionService.Set(sessionModel);

        return sessionModel.IsAlreadyPublished
            ? RedirectToRoute(RouteNames.UpdateEvent.UpdateGuestSpeakerList, new { sessionModel.CalendarEventId })!
            : RedirectToRoute(RouteNames.CreateEvent.GuestSpeakerList);
    }

    [HttpGet]
    [Route("events/new/guestspeakers/delete", Name = RouteNames.CreateEvent.GuestSpeakerDelete)]
    [Route("events/{calendarEventId}/guestspeakers/delete", Name = RouteNames.UpdateEvent.UpdateGuestSpeakerDelete)]
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

        if (sessionModel.IsAlreadyPublished)
        {
            sessionModel.HasChangedEvent = true;
        }

        _sessionService.Set(sessionModel);

        return sessionModel.IsAlreadyPublished
            ? RedirectToRoute(RouteNames.UpdateEvent.UpdateGuestSpeakerList, new { sessionModel.CalendarEventId })!
            : RedirectToRoute(RouteNames.CreateEvent.GuestSpeakerList);
    }

    [HttpGet]
    [Route("events/new/guestspeakers", Name = RouteNames.CreateEvent.GuestSpeakerList)]
    [Route("events/{calendarEventId}/guestspeakers", Name = RouteNames.UpdateEvent.UpdateGuestSpeakerList)]
    public IActionResult GetSpeakerList()
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();
        var model = GetGuestSpeakerListViewModel(sessionModel);
        return View(GuestSpeakerListViewPath, model);
    }

    [HttpPost]
    [Route("events/new/guestspeakers", Name = RouteNames.CreateEvent.GuestSpeakerList)]
    [Route("events/{calendarEventId}/guestspeakers", Name = RouteNames.UpdateEvent.UpdateGuestSpeakerList)]
    public IActionResult PostGuestSpeakerList()
    {
        var sessionModel = _sessionService.Get<EventSessionModel>();
        if (sessionModel.IsAlreadyPublished)
        {
            return RedirectToRoute(RouteNames.CalendarEvent, new { calendarEventId = sessionModel.CalendarEventId });
        }

        if (sessionModel.HasSeenPreview)
        {
            return RedirectToRoute(RouteNames.CreateEvent.CheckYourAnswers);
        }
        return RedirectToRoute(RouteNames.CreateEvent.DateAndTime);
    }

    private HasGuestSpeakersViewModel GetViewModelHasGuestSpeakers(EventSessionModel sessionModel)
    {
        var cancelLink = Url.RouteUrl(RouteNames.NetworkEvents)!;
        var postLink = Url.RouteUrl(RouteNames.CreateEvent.HasGuestSpeakers)!;

        if (sessionModel.IsAlreadyPublished)
        {
            cancelLink = Url.RouteUrl(RouteNames.CalendarEvent, new { sessionModel.CalendarEventId });
            postLink = Url.RouteUrl(RouteNames.UpdateEvent.UpdateHasGuestSpeakers,
                new { sessionModel.CalendarEventId });
        }
        else
        {
            if (sessionModel.HasSeenPreview)
            {
                cancelLink = Url.RouteUrl(RouteNames.CreateEvent.CheckYourAnswers)!;
            }
        }

        return new HasGuestSpeakersViewModel
        {
            HasGuestSpeakers = sessionModel.HasGuestSpeakers,
            CancelLink = cancelLink,
            PostLink = postLink,
            PageTitle = sessionModel.PageTitle
        };
    }

    private GuestSpeakerListViewModel GetGuestSpeakerListViewModel(EventSessionModel sessionModel)
    {
        var cancelLink = Url.RouteUrl(RouteNames.NetworkEvents)!;
        var postLink = Url.RouteUrl(RouteNames.CreateEvent.GuestSpeakerList)!;
        var addGuestSpeakerLink = Url.RouteUrl(RouteNames.CreateEvent.GuestSpeakerAdd)!;
        var deleteSpeakerLink = Url.RouteUrl(RouteNames.CreateEvent.GuestSpeakerDelete)!;

        if (sessionModel.IsAlreadyPublished)
        {
            cancelLink = Url.RouteUrl(RouteNames.CalendarEvent, new { sessionModel.CalendarEventId });
            postLink = Url.RouteUrl(RouteNames.UpdateEvent.UpdateGuestSpeakerList, new { sessionModel.CalendarEventId });
            addGuestSpeakerLink = Url.RouteUrl(RouteNames.UpdateEvent.UpdateGuestSpeakerAdd, new { sessionModel.CalendarEventId });
            deleteSpeakerLink = Url.RouteUrl(RouteNames.UpdateEvent.UpdateGuestSpeakerDelete, new { sessionModel.CalendarEventId });
        }
        else
        {
            if (sessionModel.HasSeenPreview)
            {
                cancelLink = Url.RouteUrl(RouteNames.CreateEvent.CheckYourAnswers)!;
            }
        }

        return new GuestSpeakerListViewModel
        {
            GuestSpeakers = sessionModel.GuestSpeakers,
            CancelLink = cancelLink,
            PostLink = postLink,
            AddGuestSpeakerLink = addGuestSpeakerLink!,
            DeleteSpeakerLink = deleteSpeakerLink!,
            PageTitle = sessionModel.PageTitle,
            DirectCallFromCheckYourAnswers = sessionModel.IsDirectCallFromCheckYourAnswers
        };
    }
}