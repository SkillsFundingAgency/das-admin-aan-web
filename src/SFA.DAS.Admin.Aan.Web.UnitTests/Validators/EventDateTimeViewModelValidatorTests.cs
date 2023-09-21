using FluentAssertions;
using FluentValidation.TestHelper;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;
using SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Validators;
public class EventDateTimeViewModelValidatorTests
{
    [Test]
    public void Validate_AllDetailsAreValid()
    {
        var model = GetHydratedModel();

        var sut = new EventDateTimeViewModelValidator();
        var result = sut.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void Validate_DateOfEventIsEmpty()
    {
        var model = GetHydratedModel();
        model.DateOfEvent = null;

        var sut = new EventDateTimeViewModelValidator();
        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.DateOfEvent)
            .WithErrorMessage(EventDateTimeViewModelValidator.EventDateEmpty);
    }

    [Test]
    public void Validate_DateOfEventIsInPast()
    {
        var model = GetHydratedModel();
        model.DateOfEvent = DateTime.Today.AddDays(-1);

        var sut = new EventDateTimeViewModelValidator();
        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.DateOfEvent)
            .WithErrorMessage(EventDateTimeViewModelValidator.EventDateInPast);
    }


    [TestCase(null, null, null, null, EventDateTimeViewModelValidator.EventStartHourAndMinutesEmpty, EventDateTimeViewModelValidator.EventEndHourAndMinutesEmpty)]
    [TestCase(12, null, null, null, EventDateTimeViewModelValidator.EventStartMinutesEmpty, EventDateTimeViewModelValidator.EventEndHourAndMinutesEmpty)]
    [TestCase(null, 30, null, null, EventDateTimeViewModelValidator.EventStartHourEmpty, EventDateTimeViewModelValidator.EventEndHourAndMinutesEmpty)]
    [TestCase(null, null, 13, null, EventDateTimeViewModelValidator.EventStartHourAndMinutesEmpty, EventDateTimeViewModelValidator.EventEndMinutesEmpty)]
    [TestCase(null, null, null, 0, EventDateTimeViewModelValidator.EventStartHourAndMinutesEmpty, EventDateTimeViewModelValidator.EventEndHourEmpty)]
    [TestCase(12, 30, 11, 0, null, EventDateTimeViewModelValidator.EventEndTimeBeforeStartTime)]
    [TestCase(12, 30, 1, null, null, EventDateTimeViewModelValidator.EventEndMinutesEmpty)]
    [TestCase(12, 30, null, 30, null, EventDateTimeViewModelValidator.EventEndHourEmpty)]
    [TestCase(12, null, 1, 30, EventDateTimeViewModelValidator.EventStartMinutesEmpty, null)]
    [TestCase(null, 30, 1, 30, EventDateTimeViewModelValidator.EventStartHourEmpty, null)]
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

        var sut = new EventDateTimeViewModelValidator();
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


    private static EventDateTimeViewModel GetHydratedModel()
    {
        return new EventDateTimeViewModel
        {
            DateOfEvent = DateTime.Today.AddDays(1),
            StartHour = 12,
            StartMinutes = 0,
            EndHour = 13,
            EndMinutes = 30
        };
    }
}
