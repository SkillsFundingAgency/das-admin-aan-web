using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Admin.Aan.Application.Constants;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Controllers.CreateEvent;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;
using SFA.DAS.Admin.Aan.Web.UnitTests.TestHelpers;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers.CreateEvent;
public class GuestSpeakerListControllerTests
{
    private static readonly string NetworkEventsUrl = Guid.NewGuid().ToString();
    private static readonly string PostUrl = Guid.NewGuid().ToString();

    [Test]
    public void Get_ReturnsCreateEventGuestSpeakerListViewModel()
    {
        var sessionServiceMock = new Mock<ISessionService>();

        var sessionModel = new CreateEventSessionModel
        {
            EventTitle = "title",
            EventTypeId = 1,
            EventRegionId = 2,
            EventFormat = EventFormat.Hybrid,
            GuestSpeakers = new List<GuestSpeaker>()
        };

        sessionServiceMock.Setup(s => s.Get<CreateEventSessionModel>()).Returns(sessionModel);

        var sut = new GuestSpeakersController(sessionServiceMock.Object, Mock.Of<IValidator<GuestSpeakerAddViewModel>>(), Mock.Of<IValidator<CreateEventHasGuestSpeakersViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);
        var actualResult = sut.GetSpeakerList();
        var viewResult = actualResult.As<ViewResult>();

        Assert.That(viewResult.Model, Is.TypeOf<CreateEventGuestSpeakerListViewModel>());

        ((CreateEventGuestSpeakerListViewModel)viewResult.Model!).CancelLink.Should().Be(NetworkEventsUrl);
        ((CreateEventGuestSpeakerListViewModel)viewResult.Model!).PageTitle.Should().Be(Application.Constants.CreateEvent.PageTitle);
    }

    [Test]
    public void Get_ReturnsExpectedPostLink()
    {
        var sessionServiceMock = new Mock<ISessionService>();

        var sessionModel = new CreateEventSessionModel
        {
            EventTitle = "title",
            EventTypeId = 1,
            EventRegionId = 2,
            EventFormat = EventFormat.Hybrid,
            GuestSpeakers = new List<GuestSpeaker>()
        };

        sessionServiceMock.Setup(s => s.Get<CreateEventSessionModel>()).Returns(sessionModel);

        var sut = new GuestSpeakersController(sessionServiceMock.Object, Mock.Of<IValidator<GuestSpeakerAddViewModel>>(), Mock.Of<IValidator<CreateEventHasGuestSpeakersViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.GuestSpeakerList, PostUrl);
        var actualResult = sut.GetSpeakerList();
        var viewResult = actualResult.As<ViewResult>();

        ((CreateEventGuestSpeakerListViewModel)viewResult.Model!).PostLink.Should().Be(PostUrl);
    }

    [Test]
    public void Get_ReturnsExpectedAddGuestSpeakerLink()
    {
        var sessionServiceMock = new Mock<ISessionService>();

        var sessionModel = new CreateEventSessionModel
        {
            EventTitle = "title",
            EventTypeId = 1,
            EventRegionId = 2,
            EventFormat = EventFormat.Hybrid,
            GuestSpeakers = new List<GuestSpeaker>()
        };

        sessionServiceMock.Setup(s => s.Get<CreateEventSessionModel>()).Returns(sessionModel);

        var sut = new GuestSpeakersController(sessionServiceMock.Object, Mock.Of<IValidator<GuestSpeakerAddViewModel>>(), Mock.Of<IValidator<CreateEventHasGuestSpeakersViewModel>>());

        var addGuestSpeakerLink = Guid.NewGuid().ToString();
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.GuestSpeakerAdd, addGuestSpeakerLink);
        var actualResult = sut.GetSpeakerList();
        var viewResult = actualResult.As<ViewResult>();

        ((CreateEventGuestSpeakerListViewModel)viewResult.Model!).AddGuestSpeakerLink.Should().Be(addGuestSpeakerLink);
    }

    [Test]
    public void Get_ReturnsExpectedDeleteGuestSpeakerLink()
    {
        var sessionServiceMock = new Mock<ISessionService>();

        var sessionModel = new CreateEventSessionModel
        {
            EventTitle = "title",
            EventTypeId = 1,
            EventRegionId = 2,
            EventFormat = EventFormat.Hybrid,
            GuestSpeakers = new List<GuestSpeaker>()
        };

        sessionServiceMock.Setup(s => s.Get<CreateEventSessionModel>()).Returns(sessionModel);

        var sut = new GuestSpeakersController(sessionServiceMock.Object, Mock.Of<IValidator<GuestSpeakerAddViewModel>>(), Mock.Of<IValidator<CreateEventHasGuestSpeakersViewModel>>());

        var deleteGuestSpeakerLink = Guid.NewGuid().ToString();
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.GuestSpeakerDelete, deleteGuestSpeakerLink);
        var actualResult = sut.GetSpeakerList();
        var viewResult = actualResult.As<ViewResult>();

        ((CreateEventGuestSpeakerListViewModel)viewResult.Model!).DeleteSpeakerLink.Should().Be(deleteGuestSpeakerLink);
    }

    [Test]
    public void Post_RedirectToNextPage()
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var guestSpeakers = new List<GuestSpeaker>();

        var sessionModel = new CreateEventSessionModel { GuestSpeakers = guestSpeakers };
        sessionServiceMock.Setup(s => s.Get<CreateEventSessionModel>()).Returns(sessionModel);

        var sut = new GuestSpeakersController(sessionServiceMock.Object, Mock.Of<IValidator<GuestSpeakerAddViewModel>>(), Mock.Of<IValidator<CreateEventHasGuestSpeakersViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);

        var actualResult = sut.PostGuestSpeakerList();
        var result = actualResult.As<RedirectToRouteResult>();
        sut.ModelState.IsValid.Should().BeTrue();

        result.RouteName.Should().Be(RouteNames.CreateEvent.GuestSpeakerList);
    }
}
