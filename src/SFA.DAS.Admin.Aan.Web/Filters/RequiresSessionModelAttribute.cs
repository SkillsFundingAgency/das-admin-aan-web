// using Microsoft.AspNetCore.Mvc.Controllers;
// using Microsoft.AspNetCore.Mvc.Filters;
// using SFA.DAS.Admin.Aan.Application.Services;
// using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;
// using System.Diagnostics.CodeAnalysis;
//
// namespace SFA.DAS.Admin.Aan.Web.Filters;
//
// [ExcludeFromCodeCoverage]
// public class RequiresSessionModelAttribute : ApplicationFilterAttribute
// {
//     public override void OnActionExecuting(ActionExecutingContext context)
//     {
//         if (context.ActionDescriptor is not ControllerActionDescriptor controllerActionDescriptor) return;
//
//         if (BypassCheck(controllerActionDescriptor)) return;
//
//         if (!HasValidSessionModel(context.HttpContext.RequestServices))
//         {
//             context.Result = RedirectToHome;
//         }
//     }
//
//     private bool BypassCheck(ControllerActionDescriptor controllerActionDescriptor)
//     {
//         // var controllersToByPass = new[] { nameof(BeforeYouStartController), nameof(TermsAndConditionsController), nameof(LineManagerController) };
//
//         if (!IsRequestForCreateEventAction(controllerActionDescriptor)) return true;
//
//         //if (controllersToByPass.Contains(controllerActionDescriptor.ControllerTypeInfo.Name)) return true;
//
//         return false;
//     }
//
//     private static bool HasValidSessionModel(IServiceProvider services)
//     {
//         ISessionService sessionService = services.GetService<ISessionService>()!;
//
//         CreateEventSessionModel sessionModel = sessionService.Get<CreateEventSessionModel>();
//
//         return sessionModel != null; // MFCMFC && sessionModel.IsValid;
//     }
//}
