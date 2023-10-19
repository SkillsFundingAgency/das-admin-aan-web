using FluentAssertions;
using SFA.DAS.Admin.Aan.Web.Extensions;
using SFA.DAS.DfESignIn.Auth.Constants;
using SFA.DAS.Testing.AutoFixture;
using System.Security.Claims;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Extensions
{
    public class UserExtensionsTest
    {
        [Test, MoqAutoData]
        public void GetFirstName_Returns_ExpectedResults(string firstName)
        {
            // arrange
            ClaimsIdentity identity = new();
            identity.AddClaim(new Claim(ClaimName.GivenName, firstName));
            var principal = new ClaimsPrincipal(identity);

            //sut
            var actual = principal.GetFirstName();

            //assert
            actual.Should().NotBeNullOrEmpty();
            actual.Should().Be(firstName);
        }

        [Test, MoqAutoData]
        public void GetLastName_Returns_ExpectedResults(string lastName)
        {
            // arrange
            ClaimsIdentity identity = new();
            identity.AddClaim(new Claim(ClaimName.FamilyName, lastName));
            var principal = new ClaimsPrincipal(identity);

            //sut
            var actual = principal.GetLastName();

            //assert
            actual.Should().NotBeNullOrEmpty();
            actual.Should().Be(lastName);
        }

        [Test, MoqAutoData]
        public void GetEmail_Returns_ExpectedResults(string email)
        {
            // arrange
            ClaimsIdentity identity = new();
            identity.AddClaim(new Claim(ClaimName.Email, email));
            var principal = new ClaimsPrincipal(identity);

            //sut
            var actual = principal.GetEmail();

            //assert
            actual.Should().NotBeNullOrEmpty();
            actual.Should().Be(email);
        }

        [Test, MoqAutoData]
        public void GetDisplay_Returns_ExpectedResults(string firstName, string lastName)
        {
            // arrange
            ClaimsIdentity identity = new();
            identity.AddClaim(new Claim(ClaimName.GivenName, firstName));
            identity.AddClaim(new Claim(ClaimName.FamilyName, lastName));
            var principal = new ClaimsPrincipal(identity);

            //sut
            var actual = principal.GetDisplayName();

            //assert
            actual.Should().NotBeNullOrEmpty();
            actual.Should().Be($"{firstName} {lastName}");
        }
    }
}
