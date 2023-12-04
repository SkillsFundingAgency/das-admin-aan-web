using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Aan.SharedUi.Constants;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Controllers.ManageEvent;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;
using SFA.DAS.Admin.Aan.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers.ManageEvent.GuestSpeakers;
public class GuestSpeakersControllerAddDeleteTests
{
    private static readonly string GuestSpeakerListUrl = Guid.NewGuid().ToString();
    private static readonly string GuestSpeakerAddUrl = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void Get_ReturnsSpeakerAddViewModel(
        [Frozen] Mock<IValidator<GuestSpeakerAddViewModel>> validatorMock)
    {
        var sessionServiceMock = new Mock<ISessionService>();

        var sessionModel = new EventSessionModel
        {
            EventTitle = "title",
            CalendarId = 1,
            RegionId = 2,
            EventFormat = EventFormat.Hybrid
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new GuestSpeakersController(sessionServiceMock.Object, validatorMock.Object, Mock.Of<IValidator<HasGuestSpeakersViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.GuestSpeakerList, GuestSpeakerListUrl);
        var actualResult = sut.GetAddGuestSpeaker();
        var viewResult = actualResult.As<ViewResult>();

        Assert.That(viewResult.Model, Is.TypeOf<GuestSpeakerAddViewModel>());

        ((GuestSpeakerAddViewModel)viewResult.Model!).CancelLink.Should().Be(GuestSpeakerListUrl);
        ((GuestSpeakerAddViewModel)viewResult.Model!).PageTitle.Should().Be(sessionModel.PageTitle);
    }

    [Test, MoqAutoData]
    public void Get_ReturnsSpeakerAddViewModel_WhenIsAlreadyPublishedTrue(
        [Frozen] Mock<IValidator<GuestSpeakerAddViewModel>> validatorMock)
    {
        var sessionServiceMock = new Mock<ISessionService>();

        var sessionModel = new EventSessionModel
        {
            EventTitle = "title",
            CalendarId = 1,
            RegionId = 2,
            EventFormat = EventFormat.Hybrid,
            IsAlreadyPublished = true
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new GuestSpeakersController(sessionServiceMock.Object, validatorMock.Object, Mock.Of<IValidator<HasGuestSpeakersViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.UpdateEvent.UpdateGuestSpeakerList, GuestSpeakerListUrl);
        var actualResult = sut.GetAddGuestSpeaker();
        var viewResult = actualResult.As<ViewResult>();

        Assert.That(viewResult.Model, Is.TypeOf<GuestSpeakerAddViewModel>());

        ((GuestSpeakerAddViewModel)viewResult.Model!).CancelLink.Should().Be(GuestSpeakerListUrl);
        ((GuestSpeakerAddViewModel)viewResult.Model!).PageTitle.Should().Be(sessionModel.PageTitle);
    }


    [Test, MoqAutoData]
    public void Get_ReturnsExpectedPostLink([Frozen] Mock<IValidator<GuestSpeakerAddViewModel>> validatorMock)
    {
        var sessionServiceMock = new Mock<ISessionService>();

        var sessionModel = new EventSessionModel
        {
            EventTitle = "title",
            CalendarId = 1,
            RegionId = 2,
            EventFormat = EventFormat.Hybrid
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new GuestSpeakersController(sessionServiceMock.Object, validatorMock.Object, Mock.Of<IValidator<HasGuestSpeakersViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.GuestSpeakerAdd, GuestSpeakerAddUrl);
        var actualResult = sut.GetAddGuestSpeaker();
        var viewResult = actualResult.As<ViewResult>();

        ((GuestSpeakerAddViewModel)viewResult.Model!).PostLink.Should().Be(GuestSpeakerAddUrl);
    }

    [Test, MoqAutoData]
    public void Get_ReturnsExpectedPostLink_WhenIsAlreadyPublishedTrue([Frozen] Mock<IValidator<GuestSpeakerAddViewModel>> validatorMock)
    {
        var sessionServiceMock = new Mock<ISessionService>();

        var sessionModel = new EventSessionModel
        {
            EventTitle = "title",
            CalendarId = 1,
            RegionId = 2,
            EventFormat = EventFormat.Hybrid,
            IsAlreadyPublished = true
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new GuestSpeakersController(sessionServiceMock.Object, validatorMock.Object, Mock.Of<IValidator<HasGuestSpeakersViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.UpdateEvent.UpdateGuestSpeakerAdd, GuestSpeakerAddUrl);
        var actualResult = sut.GetAddGuestSpeaker();
        var viewResult = actualResult.As<ViewResult>();

        ((GuestSpeakerAddViewModel)viewResult.Model!).PostLink.Should().Be(GuestSpeakerAddUrl);
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    public void Post_SetEventGuestListOnSessionModel(int idInCurrentList)
    {
        var name = "name 1";
        var jobRoleAndOrganisation = "job role";
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<GuestSpeakerAddViewModel>>();

        var sessionModel = new EventSessionModel
        {
            GuestSpeakers = new List<GuestSpeaker>()
        };

        if (idInCurrentList > 0)
        {
            sessionModel.GuestSpeakers.Add(new GuestSpeaker("name x", "org x", idInCurrentList));
        }

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var submitModel = new GuestSpeakerAddViewModel { Name = name, JobRoleAndOrganisation = jobRoleAndOrganisation };

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);

        var sut = new GuestSpeakersController(sessionServiceMock.Object, validatorMock.Object, Mock.Of<IValidator<HasGuestSpeakersViewModel>>());


        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.GuestSpeakerList, GuestSpeakerListUrl);

        var result = (RedirectToRouteResult)sut.PostAddGuestSpeaker(submitModel);

        sut.ModelState.IsValid.Should().BeTrue();

        if (idInCurrentList == 0)
        {
            sessionServiceMock.Verify(s =>
                s.Set(It.Is<EventSessionModel>(x => x.GuestSpeakers.Count == 1)));
        }
        else
        {
            sessionServiceMock.Verify(s =>
                s.Set(It.Is<EventSessionModel>(x => x.GuestSpeakers.Count == 2)));
        }

        sessionServiceMock.Verify(s =>
            s.Set(It.Is<EventSessionModel>(x => x.GuestSpeakers.Last().Id == idInCurrentList + 1)));

        result.RouteName.Should().Be(RouteNames.CreateEvent.GuestSpeakerList);
    }

    [Test]
    public void Post_RedirectToUpdateGuestSpeakerList_WhenIsAlreadyPublished()
    {

        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<GuestSpeakerAddViewModel>>();

        var sessionModel = new EventSessionModel
        {
            GuestSpeakers = new List<GuestSpeaker>(),
            IsAlreadyPublished = true
        };


        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var submitModel = new GuestSpeakerAddViewModel { Name = "name", JobRoleAndOrganisation = "jobRoleAndOrganisation" };

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.Validate(submitModel)).Returns(validationResult);
        var sut = new GuestSpeakersController(sessionServiceMock.Object, validatorMock.Object, Mock.Of<IValidator<HasGuestSpeakersViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.GuestSpeakerList, GuestSpeakerListUrl);

        var result = (RedirectToRouteResult)sut.PostAddGuestSpeaker(submitModel);


        result.RouteName.Should().Be(RouteNames.UpdateEvent.UpdateGuestSpeakerList);
    }

    [Test, MoqAutoData]
    public void Post_WhenValidationErrors_RedirectBackToPage(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] GuestSpeakersController sut)
    {
        sut.ModelState.AddModelError("key", "message");

        var submitModel = new GuestSpeakerAddViewModel { CancelLink = GuestSpeakerListUrl };
        var actualResult = sut.PostAddGuestSpeaker(submitModel);

        var result = actualResult.As<ViewResult>();

        sut.ModelState.IsValid.Should().BeFalse();
        Assert.That(result.Model, Is.TypeOf<GuestSpeakerAddViewModel>());
        (result.Model as GuestSpeakerAddViewModel)!.CancelLink.Should().Be(GuestSpeakerListUrl);
        sessionServiceMock.Verify(s => s.Set(It.IsAny<EventSessionModel>()), Times.Never());
    }

    [Test, MoqAutoData]
    public void Delete_GuestSpeakerRemovedFromSession(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] GuestSpeakersController sut)
    {
        var guestSpeakerList = new List<GuestSpeaker>();
        var idToRemove = 5;
        var idFirst = 1;
        var idSecond = 2;
        var guestSpeakerToRemove = new GuestSpeaker("Aaron Aardvark", "Chief", idToRemove);

        guestSpeakerList.Add(new GuestSpeaker("Betty Boop", "President", idFirst));
        guestSpeakerList.Add(guestSpeakerToRemove);
        guestSpeakerList.Add(new GuestSpeaker("Charlie Chaplin", "Bottle Washer", idSecond));

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.GuestSpeakerList, GuestSpeakerListUrl);

        var sessionModel = new EventSessionModel { GuestSpeakers = guestSpeakerList };
        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var actualResult = sut.DeleteGuestSpeaker(idToRemove);
        var result = actualResult.As<RedirectToRouteResult>();

        sut.ModelState.IsValid.Should().BeTrue();
        result.RouteName.Should().Be(RouteNames.CreateEvent.GuestSpeakerList);

        sessionServiceMock.Verify(s =>
            s.Set(It.Is<EventSessionModel>(x => x.GuestSpeakers.Count == 2)));

        sessionServiceMock.Verify(s =>
            s.Set(It.Is<EventSessionModel>(x => x.GuestSpeakers.First().Id == idFirst)));

        sessionServiceMock.Verify(s =>
            s.Set(It.Is<EventSessionModel>(x => x.GuestSpeakers.Last().Id == idSecond)));
    }

    [Test, MoqAutoData]
    public void Delete_GuestSpeakerRemovedFromSession_WhenIsAlreadyPublished(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] GuestSpeakersController sut)
    {
        var guestSpeakerList = new List<GuestSpeaker>();
        var idToRemove = 5;
        var idFirst = 1;
        var idSecond = 2;
        var guestSpeakerToRemove = new GuestSpeaker("Aaron Aardvark", "Chief", idToRemove);

        guestSpeakerList.Add(new GuestSpeaker("Betty Boop", "President", idFirst));
        guestSpeakerList.Add(guestSpeakerToRemove);
        guestSpeakerList.Add(new GuestSpeaker("Charlie Chaplin", "Bottle Washer", idSecond));

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.GuestSpeakerList, GuestSpeakerListUrl);

        var sessionModel = new EventSessionModel { GuestSpeakers = guestSpeakerList, IsAlreadyPublished = true };
        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var actualResult = sut.DeleteGuestSpeaker(idToRemove);
        var result = actualResult.As<RedirectToRouteResult>();

        sut.ModelState.IsValid.Should().BeTrue();
        result.RouteName.Should().Be(RouteNames.UpdateEvent.UpdateGuestSpeakerList);

        sessionServiceMock.Verify(s =>
            s.Set(It.Is<EventSessionModel>(x => x.GuestSpeakers.Count == 2)));
    }
}