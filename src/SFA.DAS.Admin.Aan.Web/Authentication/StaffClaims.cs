using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Admin.Aan.Web.Authentication;

[ExcludeFromCodeCoverage]
public static class StaffClaims
{
    public static readonly string UserId = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn";
    public static readonly string Surname = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname";
    public static readonly string GivenName = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname";
}
