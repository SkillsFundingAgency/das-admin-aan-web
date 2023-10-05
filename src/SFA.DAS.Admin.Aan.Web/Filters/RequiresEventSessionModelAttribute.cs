using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Admin.Aan.Web.Filters;

[ExcludeFromCodeCoverage]
public class RequiresEventSessionModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ActionDescriptor is not ControllerActionDescriptor controllerActionDescriptor) return;

        if (BypassEventCheck(controllerActionDescriptor)) return;

        if (!HasValidEventSessionModel(context.HttpContext.RequestServices))
        {
            context.Result = new RedirectToActionResult("Index", "NetworkEvents", null);
        }
    }

    private static bool HasValidEventSessionModel(IServiceProvider services)
    {
        var sessionService = services.GetService<ISessionService>()!;
        var sessionModel = sessionService.Get<EventSessionModel?>();
        return sessionModel != null;
    }

    private static bool BypassEventCheck(ControllerActionDescriptor controllerActionDescriptor)
    {
        return !controllerActionDescriptor.ControllerTypeInfo.FullName!.Contains("ManageEvent");
    }
}