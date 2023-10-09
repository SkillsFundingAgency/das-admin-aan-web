using FluentAssertions;
using FluentValidation.TestHelper;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;
using SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Validators.ManageEvents;
public class EventDateAndTimeViewModelValidatorTests
{
    [Test]
    public void Validate_AllDetailsAreValid()
    {
        var model = GetHydratedModel();

        var sut = new EventDateAndTimeViewModelValidator();
        var result = sut.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void Validate_DateOfEventIsEmpty()
    {
        var model = GetHydratedModel();
        model.DateOfEvent = null;

        var sut = new EventDateAndTimeViewModelValidator();
        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.DateOfEvent)
            .WithErrorMessage(EventDateAndTimeViewModelValidator.EventDateEmpty);
    }

    [Test]
    public void Validate_DateOfEventIsInPast()
    {
        var model = GetHydratedModel();
        model.DateOfEvent = DateTime.Today.AddDays(-1);

        var sut = new EventDateAndTimeViewModelValidator();
        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.DateOfEvent)
            .WithErrorMessage(EventDateAndTimeViewModelValidator.EventDateInPast);
    }


    [TestCase(null, null, null, null, EventDateAndTimeViewModelValidator.EventStartHourAndMinutesEmpty, EventDateAndTimeViewModelValidator.EventEndHourAndMinutesEmpty)]
    [TestCase(12, null, null, null, EventDateAndTimeViewModelValidator.EventStartMinutesEmpty, EventDateAndTimeViewModelValidator.EventEndHourAndMinutesEmpty)]
    [TestCase(null, 30, null, null, EventDateAndTimeViewModelValidator.EventStartHourEmpty, EventDateAndTimeViewModelValidator.EventEndHourAndMinutesEmpty)]
    [TestCase(null, null, 13, null, EventDateAndTimeViewModelValidator.EventStartHourAndMinutesEmpty, EventDateAndTimeViewModelValidator.EventEndMinutesEmpty)]
    [TestCase(null, null, null, 0, EventDateAndTimeViewModelValidator.EventStartHourAndMinutesEmpty, EventDateAndTimeViewModelValidator.EventEndHourEmpty)]
    [TestCase(12, 30, 11, 0, null, EventDateAndTimeViewModelValidator.EventEndTimeBeforeStartTime)]
    [TestCase(12, 30, 1, null, null, EventDateAndTimeViewModelValidator.EventEndMinutesEmpty)]
    [TestCase(12, 30, null, 30, null, EventDateAndTimeViewModelValidator.EventEndHourEmpty)]
    [TestCase(12, null, 1, 30, EventDateAndTimeViewModelValidator.EventStartMinutesEmpty, null)]
    [TestCase(null, 30, 1, 30, EventDateAndTimeViewModelValidator.EventStartHourEmpty, null)]
    [TestCase(12, 30, 13, 0, null, null)]
    [TestCase(12, 30, 12, 30, null, null)]
    public void Validate_StartAndEndTime_CheckForInvalidDetails(int? startHour, int? startMinutes, int? endHour, int? endMinutes,
        string? errorMessageForStart, string? errorMessageForEnd)
    {
        var model = GetHydratedModel();
        model.StartHour = startHour;
        model.StartMinutes = startMinutes;
        model.EndHour = endHour;
        model.EndMinutes = endMinutes;

        var sut = new EventDateAndTimeViewModelValidator();
        var result = sut.TestValidate(model);

        var numberOfErrorsExpected = 0;

        if (!string.IsNullOrEmpty(errorMessageForStart))
        {
            result.ShouldHaveValidationErrorFor(c => c.StartHour)
                .WithErrorMessage(errorMessageForStart);
            numberOfErrorsExpected++;
        }

        if (!string.IsNullOrEmpty(errorMessageForEnd))
        {
            result.ShouldHaveValidationErrorFor(c => c.EndHour)
                .WithErrorMessage(errorMessageForEnd);
            numberOfErrorsExpected++;
        }

        result.Errors.Count.Should().Be(numberOfErrorsExpected);
    }


    private static EventDateAndTimeViewModel GetHydratedModel()
    {
        return new EventDateAndTimeViewModel
        {
            DateOfEvent = DateTime.Today.AddDays(1),
            StartHour = 12,
            StartMinutes = 0,
            EndHour = 13,
            EndMinutes = 30
        };
    }
}
