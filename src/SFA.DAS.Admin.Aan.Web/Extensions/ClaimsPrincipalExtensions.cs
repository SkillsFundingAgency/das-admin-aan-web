using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace SFA.DAS.Admin.Aan.Web.Extensions;

[ExcludeFromCodeCoverage]
public static class ClaimsPrincipalExtensions
{
    public static class ClaimTypes
    {
        //MFCMFC this will probably have to change to AanAdminId ??
        public const string AanMemberId = "member_id";
    }

    public static void AddAanMemberIdClaim(this ClaimsPrincipal principal, Guid memberId)
    {
        principal.AddIdentity(new ClaimsIdentity(new[] { new Claim(ClaimTypes.AanMemberId, memberId.ToString()) }));
    }

    public static Guid GetAanMemberId(this ClaimsPrincipal principal) => GetClaimValue(principal, ClaimTypes.AanMemberId);

    private static Guid GetClaimValue(ClaimsPrincipal principal, string claimType)
    {
        var memberId = principal.FindFirstValue(claimType);
        var hasParsed = Guid.TryParse(memberId, out Guid value);
        return hasParsed ? value : Guid.Empty;
    }
}
