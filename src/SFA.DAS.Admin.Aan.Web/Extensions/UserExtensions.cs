using SFA.DAS.Admin.Aan.Web.Authentication;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using SFA.DAS.DfESignIn.Auth.Constants;
using SFA.DAS.DfESignIn.Auth.Extensions;

namespace SFA.DAS.Admin.Aan.Web.Extensions;

[ExcludeFromCodeCoverage]
public static class UserExtensions
{
    public static string GetFirstName(this ClaimsPrincipal principal) => principal.FindFirst(StaffClaims.GivenName)?.Value is null ? principal.GetClaimValue(ClaimName.GivenName) : principal.FindFirst(StaffClaims.GivenName)!.Value;

    public static string GetLastName(this ClaimsPrincipal principal) => principal.FindFirst(StaffClaims.Surname)?.Value is null ? principal.GetClaimValue(ClaimName.FamilyName) : principal.FindFirst(StaffClaims.Surname)!.Value;

    public static string GetEmail(this ClaimsPrincipal principal) => principal.FindFirst(StaffClaims.UserId)?.Value is null ? principal.GetClaimValue(ClaimName.Email) : principal.FindFirst(StaffClaims.UserId)!.Value;

    public static bool HasValidRole(this ClaimsPrincipal user) => user.IsInRole(Roles.ManageEventsRole) || user.IsInRole(Roles.ManageMembersRole);

    public static string GetDisplayName(this ClaimsPrincipal user) => $"{GetFirstName(user)} {GetLastName(user)}";
}
