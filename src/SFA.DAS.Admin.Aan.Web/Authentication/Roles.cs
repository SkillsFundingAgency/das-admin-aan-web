﻿using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace SFA.DAS.Admin.Aan.Web.Authentication;

[ExcludeFromCodeCoverage]
public static class Roles
{
    public const string RoleClaimType = "http://service/service";

    public const string ManageEventsRole = "TAD";

    public const string ManageMembersRole = "TAD";

    public static bool HasValidRole(this ClaimsPrincipal user)
    {
        return user.IsInRole(ManageEventsRole) || user.IsInRole(ManageMembersRole);
    }
}