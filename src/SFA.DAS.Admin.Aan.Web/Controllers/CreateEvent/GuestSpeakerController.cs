using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

namespace SFA.DAS.Admin.Aan.Web.Controllers.CreateEvent;
[Authorize]
[Route("manage-events/new/guest-speaker")]
public class GuestSpeakerController : Controller
{
    private readonly ISessionService _sessionService;
    private readonly IValidator<GuestSpeakerAddViewModel> _validator;

    public const string ViewPath = "~/Views/NetworkEvent/GuestSpeakerAdd.cshtml";
    public GuestSpeakerController(ISessionService sessionService, IValidator<GuestSpeakerAddViewModel> validator)
    {
        _sessionService = sessionService;
        _validator = validator;
    }

    [HttpGet]
    [Route("add", Name = RouteNames.CreateEvent.GuestSpeakerAdd)]
    public IActionResult Get(GuestSpeakerAddViewModel model)
    {
        var augmentedModel = GetViewModel(model);

        return View(ViewPath, augmentedModel);
    }


    [HttpGet]
    [Route("delete", Name = RouteNames.CreateEvent.GuestSpeakerDelete)]
    public IActionResult Delete(int id)
    {
        var sessionModel = _sessionService.Get<CreateEventSessionModel?>();
        if (sessionModel == null) return RedirectToAction("Get", "NetworkEventFormat");

        var currentGuestList = sessionModel.GuestSpeakers;
        if (currentGuestList.Any())
        {
            var removeItem = currentGuestList.First(x => x.Id == id);
            currentGuestList.Remove(removeItem);
        }

        sessionModel.GuestSpeakers = currentGuestList;
        _sessionService.Set(sessionModel);
        return RedirectToAction("Get", "GuestSpeakerList");
    }

    [HttpPost]
    [Route("add", Name = RouteNames.CreateEvent.GuestSpeakerAdd)]
    public IActionResult Post(GuestSpeakerAddViewModel submitModel)
    {
        var sessionModel = _sessionService.Get<CreateEventSessionModel?>();
        if (sessionModel == null) return RedirectToAction("Get", "NetworkEventFormat");

        var result = _validator.Validate(submitModel);

        if (!result.IsValid)
        {

            result.AddToModelState(ModelState);
            return View(ViewPath, GetViewModel(submitModel));
        }


        var currentGuestList = sessionModel.GuestSpeakers;

        var id = currentGuestList.Any() ? currentGuestList.Max(x => x.Id) + 1 : 1;


        currentGuestList.Add(new GuestSpeaker(submitModel.Name!, submitModel.JobRoleAndOrganisation!, id));
        sessionModel.GuestSpeakers = currentGuestList;

        _sessionService.Set(sessionModel);
        return RedirectToAction("Get", "GuestSpeakerList");
    }

    private GuestSpeakerAddViewModel GetViewModel(GuestSpeakerAddViewModel originalModel)
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

}