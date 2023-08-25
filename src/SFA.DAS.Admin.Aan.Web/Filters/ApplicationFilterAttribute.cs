// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.Controllers;
// using Microsoft.AspNetCore.Mvc.Filters;
// using System.Diagnostics.CodeAnalysis;
//
// namespace SFA.DAS.Admin.Aan.Web.Filters;
//
// [ExcludeFromCodeCoverage]
// public abstract class ApplicationFilterAttribute : ActionFilterAttribute
// {
//     //MFCMFC not sure what this does
//     const string DefaultActionName = "Index";
//     const string DefaultControllerName = "Home";
//     const string CreateEventFilter = "CreateEvent";
//
//     protected readonly static RedirectToActionResult RedirectToHome = new(DefaultActionName, DefaultControllerName, null);
//
//     protected static bool IsRequestForCreateEventAction(ControllerActionDescriptor action) => action.ControllerTypeInfo.FullName!.Contains(CreateEventFilter);
// }