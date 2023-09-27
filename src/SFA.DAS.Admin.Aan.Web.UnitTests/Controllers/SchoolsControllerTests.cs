using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Admin.Aan.Application.OuterApi.Schools;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Controllers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers;
public class SchoolsControllerTests
{
    [Test]
    [MoqAutoData]
    public void GetSchools_ReturnsApiResponse(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        [Greedy] SchoolsController sut,
        GetSchoolsResult expectedResult,
        string query)
    {
        outerAPiMock.Setup(o => o.GetSchools(query, It.IsAny<CancellationToken>())).ReturnsAsync(expectedResult);
        var actualResult = sut.GetSchools(query, new CancellationToken());
        actualResult.Result.As<OkObjectResult>().Value.Should().BeEquivalentTo(expectedResult.Schools);
    }
}
