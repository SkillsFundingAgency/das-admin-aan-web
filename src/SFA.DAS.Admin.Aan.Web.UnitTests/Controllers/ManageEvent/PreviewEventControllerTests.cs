﻿using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Aan.SharedUi.Models;
using SFA.DAS.Aan.SharedUi.OuterApi.Responses;
using SFA.DAS.Admin.Aan.Application.OuterApi.Calendar;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Controllers.ManageEvent;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;
using SFA.DAS.Admin.Aan.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers.ManageEvent;
internal class PreviewEventControllerTests
{
    private static readonly string CheckYourAnswersUrl = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void Get_ReturnsNetworkEventDetailsViewModel(
        [Frozen] Mock<IOuterApiClient> outerAPiMock,
        List<Calendar> calendars)
    {
        outerAPiMock.Setup(o => o.GetCalendars(It.IsAny<CancellationToken>())).ReturnsAsync(calendars);
        var sessionServiceMock = new Mock<ISessionService>();
        var guestSpeakers = new List<GuestSpeaker>
        {
            new("Joe Cool","Head of Cool",2),
            new("Mark Niac", "Head of Genius",2)
        };

        var sessionModel = new EventSessionModel
        {
            CalendarId = calendars.First().Id,
            GuestSpeakers = guestSpeakers
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new PreviewEventController(outerAPiMock.Object, sessionServiceMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.ManageEvent.CheckYourAnswers, CheckYourAnswersUrl);

        var result = sut.Get(new CancellationToken());
        var actualResult = result.Result as ViewResult;

        Assert.That(actualResult!.Model, Is.TypeOf<NetworkEventDetailsViewModel>());
        var vm = actualResult.Model as NetworkEventDetailsViewModel;
        vm!.BackLinkUrl.Should().Be(CheckYourAnswersUrl);
        vm.IsPreview.Should().BeTrue();
        vm.EventGuests.Count.Should().Be(sessionModel.GuestSpeakers.Count);

        vm.EventGuests.Should().BeEquivalentTo(sessionModel.GuestSpeakers
            .Select(guest => new EventGuest(guest.GuestName, guest.GuestJobTitle)).ToList());
    }
}
