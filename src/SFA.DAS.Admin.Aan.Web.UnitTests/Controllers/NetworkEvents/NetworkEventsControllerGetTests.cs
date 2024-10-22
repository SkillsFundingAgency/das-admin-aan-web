using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Admin.Aan.Application.OuterApi.Calendar;
using SFA.DAS.Admin.Aan.Application.OuterApi.Calendar.Responses;
using SFA.DAS.Admin.Aan.Application.OuterApi.Regions;
using SFA.DAS.Admin.Aan.Application.Services;
using SFA.DAS.Admin.Aan.Web.Controllers;
using SFA.DAS.Admin.Aan.Web.Extensions;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models.DeleteEvent;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvents;
using SFA.DAS.Admin.Aan.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Controllers.NetworkEvents;

public class NetworkEventsControllerGetTests
{
    private static readonly string AllNetworksUrl = Guid.NewGuid().ToString();

    private Mock<IOuterApiClient> _outerApiMock = null!;
    private Mock<ISessionService> _sessionServiceMock = null!;
    private Mock<IValidator<CancelEventViewModel>> _validatorMock = null!;
    private GetCalendarEventsQueryResult _result = null!;
    private DateTime? _fromDate;
    private DateTime? _toDate;
    private int CalendarId;
    private int RegionId;

    [SetUp, MoqAutoData]
    public void Setup()
    {
        CalendarId = 1;
        RegionId = 2;
        _outerApiMock = new Mock<IOuterApiClient>();

        var calendars = new List<CalendarDetail>
        {
            new() {CalendarName = "cal 1", Id = CalendarId}
        };

        var regionsResult = new GetRegionsResult
        {
            Regions = [new(1, "London", RegionId)]
        };

        _outerApiMock.Setup(o => o.GetCalendars(It.IsAny<CancellationToken>())).ReturnsAsync(calendars);
        _outerApiMock.Setup(o => o.GetRegions(It.IsAny<CancellationToken>())).ReturnsAsync(regionsResult);

        _sessionServiceMock = new Mock<ISessionService>();
        var sessionModel = new EventSessionModel();

        _sessionServiceMock.Setup(s => s.Get<EventSessionModel>()).Returns(sessionModel);

        _validatorMock = new Mock<IValidator<CancelEventViewModel>>();

        var validationResult = new ValidationResult();
        _validatorMock.Setup(v => v.Validate(It.IsAny<CancelEventViewModel>())).Returns(validationResult);

        _result = new GetCalendarEventsQueryResult
        {
            CalendarEvents = new List<CalendarEventSummary>(),
            Page = 1,
            PageSize = 10,
            TotalCount = 55,
            TotalPages = 6
        };

        _fromDate = DateTime.Now.AddDays(1);
        _toDate = DateTime.Now.AddDays(2);
    }

    [Test, AutoData]
    public async Task GetCalendarEvents_AppendsLinks(string cancelEventUrl, string editEventUrl, string eventDetailsUrl, GetCalendarEventsQueryResult apiResponse)
    {
        _outerApiMock
            .Setup(o => o.GetCalendarEvents(It.IsAny<Guid>(), It.IsAny<Dictionary<string, string[]>>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(apiResponse);

        var sut = new NetworkEventsController(_outerApiMock.Object, _sessionServiceMock.Object, _validatorMock.Object);
        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.NetworkEvents, AllNetworksUrl)
            .AddUrlForRoute(RouteNames.DeleteEvent, cancelEventUrl)
            .AddUrlForRoute(RouteNames.CalendarEvent, editEventUrl)
            .AddUrlForRoute(RouteNames.EventDetails, eventDetailsUrl);

        var actualResult = await sut.Index(new GetNetworkEventsRequest(), new CancellationToken());

        var model = actualResult.As<ViewResult>().Model.As<NetworkEventsViewModel>();
        model.CalendarEvents.Should().Contain(c => c.EditEventLink == editEventUrl && c.CancelEventLink == cancelEventUrl && c.ViewDetailsLink == eventDetailsUrl);
    }

    [Test]
    public void GetCalendarEvents_ReturnsApiResponse()
    {
        var request = new GetNetworkEventsRequest
        {
            Page = _result.Page,
            PageSize = _result.PageSize,
            FromDate = _fromDate,
            ToDate = _toDate,
            CalendarId = [CalendarId],
            IsActive = [true]
        };

        _outerApiMock
            .Setup(o => o.GetCalendarEvents(It.IsAny<Guid>(), It.IsAny<Dictionary<string, string[]>>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(_result);

        var sut = new NetworkEventsController(_outerApiMock.Object, _sessionServiceMock.Object, _validatorMock.Object);
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, AllNetworksUrl);

        var actualResult = sut.Index(request, new CancellationToken());

        var viewResult = actualResult.Result.As<ViewResult>();
        var model = viewResult.Model as NetworkEventsViewModel;
        model!.PaginationViewModel.CurrentPage.Should().Be(_result.Page);
        model!.PaginationViewModel.PageSize.Should().Be(_result.PageSize);
        model!.PaginationViewModel.TotalPages.Should().Be(_result.TotalPages);
        model!.TotalCount.Should().Be(_result.TotalCount);

        model.Should().BeEquivalentTo(_result, options => options.ExcludingMissingMembers());

        _outerApiMock.Verify(
            o => o.GetCalendarEvents(It.IsAny<Guid>(), It.IsAny<Dictionary<string, string[]>>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void GetCalendarEventsWithInvalidLocation_ReturnsViewModelWithInvalidLocation()
    {
        var request = new GetNetworkEventsRequest
        {
            CalendarId = [CalendarId],
            IsActive = [true]
        };

        var apiResult = new GetCalendarEventsQueryResult
        {
            TotalCount = 0,
            IsInvalidLocation = true
        };

        _outerApiMock
            .Setup(o => o.GetCalendarEvents(It.IsAny<Guid>(), It.IsAny<Dictionary<string, string[]>>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(apiResult);

        var sut = new NetworkEventsController(_outerApiMock.Object, _sessionServiceMock.Object, _validatorMock.Object);
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, AllNetworksUrl);

        var actualResult = sut.Index(request, new CancellationToken());

        var viewResult = actualResult.Result.As<ViewResult>();
        var model = viewResult.Model as NetworkEventsViewModel;
        model!.IsInvalidLocation.Should().BeTrue();
        model!.TotalCount.Should().Be(0);

        _outerApiMock.Verify(
            o => o.GetCalendarEvents(It.IsAny<Guid>(), It.IsAny<Dictionary<string, string[]>>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test, MoqAutoData]
    public void GetCalendarEvents_ReturnsApiResponse_CheckDateFilterChoices()
    {
        var fromDateFormatted = _fromDate?.ToString("yyyy-MM-dd")!;
        var toDateFormatted = _toDate?.ToString("yyyy-MM-dd")!;

        var request = new GetNetworkEventsRequest
        {
            FromDate = _fromDate,
            ToDate = _toDate
        };

        _outerApiMock
            .Setup(o => o.GetCalendarEvents(It.IsAny<Guid>(), It.IsAny<Dictionary<string, string[]>>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(_result);

        var sut = new NetworkEventsController(_outerApiMock.Object, _sessionServiceMock.Object, _validatorMock.Object);
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, AllNetworksUrl);

        var actualResult = sut.Index(request, new CancellationToken());

        var viewResult = actualResult.Result.As<ViewResult>();
        var model = viewResult.Model as NetworkEventsViewModel;

        model!.FilterChoices.FromDate?.ToApiString().Should().Be(fromDateFormatted);
        model!.FilterChoices.ToDate?.ToApiString().Should().Be(toDateFormatted);
    }

    [TestCase(true, true)]
    [TestCase(true, false)]
    [TestCase(false, true)]
    [TestCase(false, false)]

    public void GetCalendarEventsWithEventStatusCheck_ReturnsApiResponse(bool isPublishedTicked, bool isCancelledTicked)
    {

        var request = new GetNetworkEventsRequest
        {
            Page = 1,
            PageSize = 1
        };

        if (isPublishedTicked)
            request.IsActive.Add(true);
        if (isCancelledTicked)
            request.IsActive.Add(false);


        var expectedResult = new GetCalendarEventsQueryResult();

        var outerApiMock = new Mock<IOuterApiClient>();
        var sessionServiceMock = new Mock<ISessionService>();
        outerApiMock
            .Setup(o => o.GetCalendarEvents(It.IsAny<Guid>(), It.IsAny<Dictionary<string, string[]>>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(expectedResult);
        outerApiMock.Setup(o => o.GetCalendars(It.IsAny<CancellationToken>())).ReturnsAsync([]);
        outerApiMock.Setup(o => o.GetRegions(It.IsAny<CancellationToken>())).ReturnsAsync(new GetRegionsResult());

        var sut = new NetworkEventsController(outerApiMock.Object, sessionServiceMock.Object, Mock.Of<IValidator<CancelEventViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.NetworkEvents, AllNetworksUrl);

        var actualResult = sut.Index(request, new CancellationToken());

        var viewResult = actualResult.Result.As<ViewResult>();
        var model = viewResult.Model as NetworkEventsViewModel;

        model!.ClearSelectedFiltersLink.Should().Be(AllNetworksUrl);

        switch (isPublishedTicked)
        {
            case true:
                model!.FilterChoices.EventStatusChecklistDetails.Lookups.First(x => x.Name == "Published").Checked
                    .Should().NotBeEmpty();
                break;
            case false:
                model!.FilterChoices.EventStatusChecklistDetails.Lookups.First(x => x.Name == "Published").Checked
                    .Should().BeEmpty();
                break;
        }

        switch (isCancelledTicked)
        {
            case true:
                model!.FilterChoices.EventStatusChecklistDetails.Lookups.First(x => x.Name == "Cancelled").Checked
                    .Should().NotBeEmpty();
                break;
            case false:
                model!.FilterChoices.EventStatusChecklistDetails.Lookups.First(x => x.Name == "Cancelled").Checked
                    .Should().BeEmpty();
                break;
        }

        sessionServiceMock.Verify(s => s.Delete(nameof(EventSessionModel)), Times.Once);
    }

    [Test]
    public void CreateEvent_BuildSessionModelAndRerouteToCreateEventFormat()
    {
        var sessionServiceMock = new Mock<ISessionService>();

        var sut = new NetworkEventsController(Mock.Of<IOuterApiClient>(), sessionServiceMock.Object, Mock.Of<IValidator<CancelEventViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CreateEvent.CreateNewEvent, AllNetworksUrl);

        var actualResult = sut.CreateEvent();

        var result = (RedirectToRouteResult)actualResult;

        sessionServiceMock.Verify(s => s.Set(It.IsAny<EventSessionModel>()), Times.Once());
        result.RouteName.Should().Be(RouteNames.CreateEvent.EventFormat);
    }
}