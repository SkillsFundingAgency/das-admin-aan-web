﻿using AutoFixture.NUnit3;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Aan.SharedUi.Models;
using SFA.DAS.Aan.SharedUi.OuterApi.Responses;
using SFA.DAS.Admin.Aan.Application.OuterApi.Calendar;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Controllers;
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
        List<CalendarDetail> calendars,
        List<GuestSpeaker> guestSpeakers)
    {
        outerAPiMock.Setup(o => o.GetCalendars(It.IsAny<CancellationToken>())).ReturnsAsync(calendars);
        var sessionServiceMock = new Mock<ISessionService>();

        var sessionModel = new EventSessionModel
        {
            CalendarId = calendars.First().Id,
            GuestSpeakers = guestSpeakers
        };

        sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        var sut = new PreviewEventController(outerAPiMock.Object, sessionServiceMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.CheckYourAnswers, CheckYourAnswersUrl);

        var result = sut.Get(new CancellationToken());
        var actualResult = result.Result.As<ViewResult>();

        using (new AssertionScope())
        {
            actualResult.Model.Should().BeOfType<NetworkEventDetailsViewModel>();
            var vm = actualResult.Model.As<NetworkEventDetailsViewModel>();
            vm.BackLinkUrl.Should().Be(CheckYourAnswersUrl);
            vm.BackLinkDescription.Should().Be(CalendarEventController.PreviewBackLinkDescription);
            vm.PreviewHeader.Should().Be(CalendarEventController.EventPreviewHeader);
            vm.IsActive.Should().BeTrue();
            vm.IsPreview.Should().BeTrue();
            vm.EventGuests.Count.Should().Be(sessionModel.GuestSpeakers.Count);
            vm.EventGuests.Should().BeEquivalentTo(sessionModel.GuestSpeakers
                .Select(guest => new EventGuest(guest.GuestName, guest.GuestJobTitle)).ToList());
        }
    }
}
