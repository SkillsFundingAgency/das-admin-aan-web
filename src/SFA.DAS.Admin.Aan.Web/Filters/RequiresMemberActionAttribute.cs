using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using SFA.DAS.Admin.Aan.Application.Constants;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Extensions;

namespace SFA.DAS.Admin.Aan.Web.Filters;

[ExcludeFromCodeCoverage]
public class RequiresMemberActionAttribute : ActionFilterAttribute
{
    private readonly IOuterApiClient _outerApiClient;
    private readonly ISessionService _sessionService;

    public RequiresMemberActionAttribute(IOuterApiClient outerApiClient, ISessionService sessionService)
    {
        _outerApiClient = outerApiClient;
        _sessionService = sessionService;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ActionDescriptor is not ControllerActionDescriptor) return;

        if (context.HttpContext.User.HasValidRole())
        {
            var memberId = _sessionService.Get(SessionKeys.MemberId);

            if (string.IsNullOrEmpty(memberId))
            {
                var member = await _outerApiClient.GetAdminMember(new(context.HttpContext.User.GetEmail(), context.HttpContext.User.GetFirstName(), context.HttpContext.User.GetLastName()), CancellationToken.None);
                _sessionService.Set(SessionKeys.MemberId, member.MemberId.ToString());
            }
        }

        await next();
    }
}
