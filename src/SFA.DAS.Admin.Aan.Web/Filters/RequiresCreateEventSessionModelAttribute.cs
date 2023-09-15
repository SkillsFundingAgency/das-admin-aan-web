using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Controllers.CreateEvent;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Admin.Aan.Web.Filters;

[ExcludeFromCodeCoverage]
public class RequiresCreateEventSessionModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ActionDescriptor is not ControllerActionDescriptor controllerActionDescriptor) return;

        if (BypassCreateEventCheck(controllerActionDescriptor)) return;

        if (!HasValidCreateEventSessionModel(context.HttpContext.RequestServices))
        {
            context.Result = new RedirectToActionResult("Index", "NetworkEvents", null);
        }
    }

    private static bool HasValidCreateEventSessionModel(IServiceProvider services)
    {
        var sessionService = services.GetService<ISessionService>()!;
        var sessionModel = sessionService.Get<CreateEventSessionModel>();
        return sessionModel != null!;
    }

    private static bool BypassCreateEventCheck(ControllerActionDescriptor controllerActionDescriptor)
    {
        var createEventControllers = new[]
        {
            nameof(NetworkEventDescriptionController),
            nameof(NetworkEventTypeController),
            nameof(GuestSpeakersController)
        };

        return !createEventControllers.Contains(controllerActionDescriptor.ControllerTypeInfo.Name);
    }
}