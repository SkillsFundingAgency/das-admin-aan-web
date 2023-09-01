using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Controllers.CreateEvent;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;
using SFA.DAS.Admin.Aan.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers;
public class NetworkEventTypeControllerTests
{
    private static readonly string Url = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void Get_ReturnsCreateEventTypeViewModel(
        [Frozen] Mock<IOuterApiClient> outerApiMock,
        [Greedy] NetworkEventTypeController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, Url);
        var actualResult = sut.Get(new CancellationToken());
        var viewResult = actualResult.Result.As<ViewResult>();

        outerApiMock.Verify(o => o.GetCalendars(It.IsAny<CancellationToken>()), Times.Once);
        outerApiMock.Verify(o => o.GetRegions(It.IsAny<CancellationToken>()), Times.Once);
        Assert.That(viewResult.Model, Is.TypeOf<CreateEventTypeViewModel>());

        ((CreateEventTypeViewModel)viewResult.Model!).BackLink.Should().Be(Url);
    }
}
