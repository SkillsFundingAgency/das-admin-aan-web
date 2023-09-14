using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using SFA.DAS.Admin.Aan.Web.Authentication;

namespace SFA.DAS.Admin.Aan.Web.Extensions;

[ExcludeFromCodeCoverage]
public static class UserExtensions
{
    public static string GetFirstName(this ClaimsPrincipal principal) => principal.FindFirst(StaffClaims.GivenName)!.Value;
    public static string GetLastName(this ClaimsPrincipal principal) => principal.FindFirst(StaffClaims.Surname)!.Value;
    public static string GetEmail(this ClaimsPrincipal principal) => principal.FindFirst(StaffClaims.UserId)!.Value;
}
