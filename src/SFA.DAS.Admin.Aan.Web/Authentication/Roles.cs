using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Admin.Aan.Web.Authentication;

[ExcludeFromCodeCoverage]
public static class Roles
{
    public const string RoleClaimType = "http://service/service";

    public const string ManageEventsRole = "AME";

    public const string ManageMembersRole = "AMM";
}
