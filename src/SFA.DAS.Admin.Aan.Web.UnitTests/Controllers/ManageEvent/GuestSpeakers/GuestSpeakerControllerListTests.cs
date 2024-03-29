﻿using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Aan.SharedUi.Constants;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Controllers.ManageEvent;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;
using SFA.DAS.Admin.Aan.Web.UnitTests.TestHelpers;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers.ManageEvent.GuestSpeakers;
public class GuestSpeakerControllerListTests
{
    private static readonly string NetworkEventsUrl = Guid.NewGuid().ToString();
    private static readonly string PostUrl = Guid.NewGuid().ToString();
    private static readonly string CancelUrl = Guid.NewGuid().ToString();
    private static readonly string CheckYourAnswersUrl = Guid.NewGuid().ToString();
    private static readonly string CalendarEventUrl = Guid.NewGuid().ToString();

    [Test]
    public void Get_ReturnsEventGuestSpeakerListViewModel()
    {
        var sessionServiceMock = new Mock<ISessionService>();

        var sessionModel = new EventSessionModel
        {
            EventTitle = "title",
            CalendarId = 1,
            RegionId = 2,
            EventFormat = EventFormat.Hybrid,
            GuestSpeakers = []
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new GuestSpeakersController(sessionServiceMock.Object, Mock.Of<IValidator<GuestSpeakerAddViewModel>>(), Mock.Of<IValidator<HasGuestSpeakersViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);
        var actualResult = sut.GetSpeakerList();
        var viewResult = actualResult.As<ViewResult>();

        Assert.That(viewResult.Model, Is.TypeOf<GuestSpeakerListViewModel>());

        ((GuestSpeakerListViewModel)viewResult.Model!).CancelLink.Should().Be(NetworkEventsUrl);
        ((GuestSpeakerListViewModel)viewResult.Model!).PageTitle.Should().Be(Application.Constants.CreateEvent.PageTitle);
    }

    [Test]
    public void Get_ReturnsExpectedPostLink()
    {
        var sessionServiceMock = new Mock<ISessionService>();

        var sessionModel = new EventSessionModel
        {
            EventTitle = "title",
            CalendarId = 1,
            RegionId = 2,
            EventFormat = EventFormat.Hybrid,
            GuestSpeakers = []
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new GuestSpeakersController(sessionServiceMock.Object, Mock.Of<IValidator<GuestSpeakerAddViewModel>>(), Mock.Of<IValidator<HasGuestSpeakersViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.GuestSpeakerList, PostUrl);
        var actualResult = sut.GetSpeakerList();
        var viewResult = actualResult.As<ViewResult>();

        ((GuestSpeakerListViewModel)viewResult.Model!).PostLink.Should().Be(PostUrl);
    }

    [Test]
    public void Get_ReturnsExpectedPostLink_WhenIsAlreadyPublishedTrue()
    {
        var sessionServiceMock = new Mock<ISessionService>();

        var sessionModel = new EventSessionModel
        {
            EventTitle = "title",
            CalendarId = 1,
            RegionId = 2,
            EventFormat = EventFormat.Hybrid,
            GuestSpeakers = [],
            IsAlreadyPublished = true
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new GuestSpeakersController(sessionServiceMock.Object, Mock.Of<IValidator<GuestSpeakerAddViewModel>>(), Mock.Of<IValidator<HasGuestSpeakersViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.UpdateEvent.UpdateGuestSpeakerList, PostUrl);
        var actualResult = sut.GetSpeakerList();
        var viewResult = actualResult.As<ViewResult>();

        ((GuestSpeakerListViewModel)viewResult.Model!).PostLink.Should().Be(PostUrl);
    }

    [Test]
    public void Get_ReturnsExpectedCancelLink()
    {
        var sessionServiceMock = new Mock<ISessionService>();

        var sessionModel = new EventSessionModel
        {
            EventTitle = "title",
            CalendarId = 1,
            RegionId = 2,
            EventFormat = EventFormat.Hybrid,
            GuestSpeakers = [],
            HasSeenPreview = false
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new GuestSpeakersController(sessionServiceMock.Object, Mock.Of<IValidator<GuestSpeakerAddViewModel>>(), Mock.Of<IValidator<HasGuestSpeakersViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, CancelUrl);
        var actualResult = sut.GetSpeakerList();
        var viewResult = actualResult.As<ViewResult>();

        ((GuestSpeakerListViewModel)viewResult.Model!).CancelLink.Should().Be(CancelUrl);
    }

    [Test]
    public void Get_ReturnsExpectedCancelLink_WhenIsAlreadyPublishedTrue()
    {
        var sessionServiceMock = new Mock<ISessionService>();

        var sessionModel = new EventSessionModel
        {
            EventTitle = "title",
            CalendarId = 1,
            RegionId = 2,
            EventFormat = EventFormat.Hybrid,
            GuestSpeakers = [],
            HasSeenPreview = false,
            IsAlreadyPublished = true
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new GuestSpeakersController(sessionServiceMock.Object, Mock.Of<IValidator<GuestSpeakerAddViewModel>>(), Mock.Of<IValidator<HasGuestSpeakersViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CalendarEvent, CancelUrl);
        var actualResult = sut.GetSpeakerList();
        var viewResult = actualResult.As<ViewResult>();

        ((GuestSpeakerListViewModel)viewResult.Model!).CancelLink.Should().Be(CancelUrl);
    }

    [Test]
    public void Get_ReturnsExpectedCancelLinkToBeCheckYourAnswers_HasSeenPreviewIsTrue()
    {
        var sessionServiceMock = new Mock<ISessionService>();

        var sessionModel = new EventSessionModel
        {
            EventTitle = "title",
            CalendarId = 1,
            RegionId = 2,
            EventFormat = EventFormat.Hybrid,
            GuestSpeakers = [],
            HasSeenPreview = true
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new GuestSpeakersController(sessionServiceMock.Object, Mock.Of<IValidator<GuestSpeakerAddViewModel>>(), Mock.Of<IValidator<HasGuestSpeakersViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.CheckYourAnswers, CheckYourAnswersUrl);
        var actualResult = sut.GetSpeakerList();
        var viewResult = actualResult.As<ViewResult>();

        ((GuestSpeakerListViewModel)viewResult.Model!).CancelLink.Should().Be(CheckYourAnswersUrl);
    }

    [Test]
    public void Get_ReturnsExpectedAddGuestSpeakerLink()
    {
        var sessionServiceMock = new Mock<ISessionService>();

        var sessionModel = new EventSessionModel
        {
            EventTitle = "title",
            CalendarId = 1,
            RegionId = 2,
            EventFormat = EventFormat.Hybrid,
            GuestSpeakers = []
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new GuestSpeakersController(sessionServiceMock.Object, Mock.Of<IValidator<GuestSpeakerAddViewModel>>(), Mock.Of<IValidator<HasGuestSpeakersViewModel>>());

        var addGuestSpeakerLink = Guid.NewGuid().ToString();
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.GuestSpeakerAdd, addGuestSpeakerLink);
        var actualResult = sut.GetSpeakerList();
        var viewResult = actualResult.As<ViewResult>();

        ((GuestSpeakerListViewModel)viewResult.Model!).AddGuestSpeakerLink.Should().Be(addGuestSpeakerLink);
    }

    [Test]
    public void Get_ReturnsExpectedAddGuestSpeakerLink_WhenIsAlreadyPublishedTrue()
    {
        var sessionServiceMock = new Mock<ISessionService>();

        var sessionModel = new EventSessionModel
        {
            EventTitle = "title",
            CalendarId = 1,
            RegionId = 2,
            EventFormat = EventFormat.Hybrid,
            GuestSpeakers = [],
            IsAlreadyPublished = true
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new GuestSpeakersController(sessionServiceMock.Object, Mock.Of<IValidator<GuestSpeakerAddViewModel>>(), Mock.Of<IValidator<HasGuestSpeakersViewModel>>());

        var addGuestSpeakerLink = Guid.NewGuid().ToString();
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.UpdateEvent.UpdateGuestSpeakerAdd, addGuestSpeakerLink);
        var actualResult = sut.GetSpeakerList();
        var viewResult = actualResult.As<ViewResult>();

        ((GuestSpeakerListViewModel)viewResult.Model!).AddGuestSpeakerLink.Should().Be(addGuestSpeakerLink);
    }

    [Test]
    public void Get_ReturnsExpectedDeleteGuestSpeakerLink()
    {
        var sessionServiceMock = new Mock<ISessionService>();

        var sessionModel = new EventSessionModel
        {
            EventTitle = "title",
            CalendarId = 1,
            RegionId = 2,
            EventFormat = EventFormat.Hybrid,
            GuestSpeakers = []
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new GuestSpeakersController(sessionServiceMock.Object, Mock.Of<IValidator<GuestSpeakerAddViewModel>>(), Mock.Of<IValidator<HasGuestSpeakersViewModel>>());

        var deleteGuestSpeakerLink = Guid.NewGuid().ToString();
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.GuestSpeakerDelete, deleteGuestSpeakerLink);
        var actualResult = sut.GetSpeakerList();
        var viewResult = actualResult.As<ViewResult>();

        ((GuestSpeakerListViewModel)viewResult.Model!).DeleteSpeakerLink.Should().Be(deleteGuestSpeakerLink);
    }

    [Test]
    public void Get_ReturnsExpectedDeleteGuestSpeakerLink_WhenIsAlreadyPublIshedTrue()
    {
        var sessionServiceMock = new Mock<ISessionService>();

        var sessionModel = new EventSessionModel
        {
            EventTitle = "title",
            CalendarId = 1,
            RegionId = 2,
            EventFormat = EventFormat.Hybrid,
            GuestSpeakers = [],
            IsAlreadyPublished = true
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new GuestSpeakersController(sessionServiceMock.Object, Mock.Of<IValidator<GuestSpeakerAddViewModel>>(), Mock.Of<IValidator<HasGuestSpeakersViewModel>>());

        var deleteGuestSpeakerLink = Guid.NewGuid().ToString();
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.UpdateEvent.UpdateGuestSpeakerDelete, deleteGuestSpeakerLink);
        var actualResult = sut.GetSpeakerList();
        var viewResult = actualResult.As<ViewResult>();

        ((GuestSpeakerListViewModel)viewResult.Model!).DeleteSpeakerLink.Should().Be(deleteGuestSpeakerLink);
    }

    [Test]
    public void Post_RerouteToEventDateTime()
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var guestSpeakers = new List<GuestSpeaker>();

        var sessionModel = new EventSessionModel { GuestSpeakers = guestSpeakers, HasSeenPreview = false, IsAlreadyPublished = false };
        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new GuestSpeakersController(sessionServiceMock.Object, Mock.Of<IValidator<GuestSpeakerAddViewModel>>(), Mock.Of<IValidator<HasGuestSpeakersViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, NetworkEventsUrl);

        var actualResult = sut.PostGuestSpeakerList();
        var result = actualResult.As<RedirectToRouteResult>();
        sut.ModelState.IsValid.Should().BeTrue();

        result.RouteName.Should().Be(RouteNames.CreateEvent.DateAndTime);
    }

    [Test]
    public void Post_RedirectToCalendarEvent_WhenIsAlreadyPublishedTrue()
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var guestSpeakers = new List<GuestSpeaker>();

        var sessionModel = new EventSessionModel { GuestSpeakers = guestSpeakers, HasSeenPreview = false, IsAlreadyPublished = true };
        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new GuestSpeakersController(sessionServiceMock.Object, Mock.Of<IValidator<GuestSpeakerAddViewModel>>(), Mock.Of<IValidator<HasGuestSpeakersViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CalendarEvent, CalendarEventUrl);

        var actualResult = sut.PostGuestSpeakerList();
        var result = actualResult.As<RedirectToRouteResult>();
        sut.ModelState.IsValid.Should().BeTrue();

        result.RouteName.Should().Be(RouteNames.CalendarEvent);
    }

    [Test]
    public void Post_HasSeenPreview_RerouteToCheckYourAnswers()
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var guestSpeakers = new List<GuestSpeaker>();

        var sessionModel = new EventSessionModel { GuestSpeakers = guestSpeakers, HasSeenPreview = true };
        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new GuestSpeakersController(sessionServiceMock.Object, Mock.Of<IValidator<GuestSpeakerAddViewModel>>(), Mock.Of<IValidator<HasGuestSpeakersViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.CheckYourAnswers, CheckYourAnswersUrl);

        var actualResult = sut.PostGuestSpeakerList();
        var result = actualResult.As<RedirectToRouteResult>();
        sut.ModelState.IsValid.Should().BeTrue();

        result.RouteName.Should().Be(RouteNames.CreateEvent.CheckYourAnswers);
    }
}