using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Admin.Aan.Application.Constants;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Controllers.CreateEvent;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;
using SFA.DAS.Admin.Aan.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers.CreateEvent;
public class GuestSpeakerControllerTests
{
    private static readonly string GuestSpeakerListUrl = Guid.NewGuid().ToString();
    private static readonly string GuestSpeakerAddUrl = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void Get_ReturnsCreateEventTypeViewModel(
        [Frozen] Mock<IValidator<GuestSpeakerAddViewModel>> validatorMock)
    {
        var sessionServiceMock = new Mock<ISessionService>();

        var sessionModel = new CreateEventSessionModel
        {
            EventTitle = "title",
            EventTypeId = 1,
            EventRegionId = 2,
            EventFormat = EventFormat.Hybrid
        };

        var model = new GuestSpeakerAddViewModel();

        sessionServiceMock.Setup(s => s.Get<CreateEventSessionModel>()).Returns(sessionModel);

        var sut = new GuestSpeakerController(sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.GuestSpeakerList, GuestSpeakerListUrl);
        var actualResult = sut.Get(model);
        var viewResult = actualResult.As<ViewResult>();

        Assert.That(viewResult.Model, Is.TypeOf<GuestSpeakerAddViewModel>());

        ((GuestSpeakerAddViewModel)viewResult.Model!).CancelLink.Should().Be(GuestSpeakerListUrl);
        ((GuestSpeakerAddViewModel)viewResult.Model!).PageTitle.Should().Be(Application.Constants.CreateEvent.PageTitle);
    }

    [Test, MoqAutoData]
    public void Get_ReturnsExpectedPostLink([Frozen] Mock<IValidator<GuestSpeakerAddViewModel>> validatorMock)
    {
        var sessionServiceMock = new Mock<ISessionService>();

        var sessionModel = new CreateEventSessionModel
        {
            EventTitle = "title",
            EventTypeId = 1,
            EventRegionId = 2,
            EventFormat = EventFormat.Hybrid
        };

        sessionServiceMock.Setup(s => s.Get<CreateEventSessionModel>()).Returns(sessionModel);
        var model = new GuestSpeakerAddViewModel();

        var sut = new GuestSpeakerController(sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.GuestSpeakerAdd, GuestSpeakerAddUrl);
        var actualResult = sut.Get(model);
        var viewResult = actualResult.As<ViewResult>();

        ((GuestSpeakerAddViewModel)viewResult.Model!).PostLink.Should().Be(GuestSpeakerAddUrl);
    }

    [Test]
    public void Post_SetGuestSpeakerAddOnNoSessionModel()
    {
        var sessionServiceMock = new Mock<ISessionService>();
        var validatorMock = new Mock<IValidator<GuestSpeakerAddViewModel>>();
        var sessionModel = new CreateEventSessionModel();

        var submitModel = new GuestSpeakerAddViewModel();

        sessionServiceMock.Setup(s => s.Get<CreateEventSessionModel>()).Returns((CreateEventSessionModel)null!);

        var validationResult = new ValidationResult();
        validatorMock.Setup(v => v.ValidateAsync(submitModel, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);

        var sut = new GuestSpeakerController(sessionServiceMock.Object, validatorMock.Object);

        var actualResult = sut.Post(submitModel);

        var result = actualResult.As<RedirectToActionResult>();

        sut.ModelState.IsValid.Should().BeTrue();

        result.ControllerName.Should().Be("NetworkEventFormat");
        result.ActionName.Should().Be("Get");
    }

    [Test, MoqAutoData]
    public void Post_WhenValidationErrors_RedirectToEventFormat(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] GuestSpeakerController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.GuestSpeakerList, GuestSpeakerListUrl);

        var sessionModel = new CreateEventSessionModel();
        sessionServiceMock.Setup(s => s.Get<CreateEventSessionModel>()).Returns(sessionModel);

        sut.ModelState.AddModelError("key", "message");

        var submitModel = new GuestSpeakerAddViewModel();
        var actualResult = sut.Post(submitModel);

        var result = actualResult.As<ViewResult>();

        sut.ModelState.IsValid.Should().BeFalse();
        Assert.That(result.Model, Is.TypeOf<GuestSpeakerAddViewModel>());
        (result.Model as GuestSpeakerAddViewModel)!.CancelLink.Should().Be(GuestSpeakerListUrl);
    }

    [Test, MoqAutoData]
    public void Delete_GuestSpeakerRemovedFromSession(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] GuestSpeakerController sut)
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

        var sessionModel = new CreateEventSessionModel { GuestSpeakers = guestSpeakerList };
        sessionServiceMock.Setup(s => s.Get<CreateEventSessionModel>()).Returns(sessionModel);

        var actualResult = sut.Delete(idToRemove);
        var result = actualResult.As<RedirectToActionResult>();

        sut.ModelState.IsValid.Should().BeTrue();
        result.ControllerName.Should().Be("GuestSpeakerList");
        result.ActionName.Should().Be("Get");

        sessionServiceMock.Verify(s =>
            s.Set(It.Is<CreateEventSessionModel>(x => x.GuestSpeakers.Count == 2)));

        sessionServiceMock.Verify(s =>
            s.Set(It.Is<CreateEventSessionModel>(x => x.GuestSpeakers.First().Id == idFirst)));

        sessionServiceMock.Verify(s =>
            s.Set(It.Is<CreateEventSessionModel>(x => x.GuestSpeakers.Last().Id == idSecond)));
    }
}

