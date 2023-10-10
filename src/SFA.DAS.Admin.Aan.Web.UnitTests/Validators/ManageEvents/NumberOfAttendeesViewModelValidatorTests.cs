using FluentAssertions;
using FluentValidation.TestHelper;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;
using SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Validators.ManageEvents;
public class NumberOfEventsViewModelValidatorTests
{
    [Test]
    public void Validate_NumberOfAttendeesIsValid()
    {
        var model = new NumberOfAttendeesViewModel { NumberOfAttendees = 1 };
        var sut = new NumberOfAttendeesViewModelValidator();
        var result = sut.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void Validate_NumberOfAttendees_NotSet()
    {
        var model = new NumberOfAttendeesViewModel { NumberOfAttendees = null };

        var sut = new NumberOfAttendeesViewModelValidator();
        var result = sut.TestValidate(model);

        var numberOfErrorsExpected = 1;
        result.Errors.Count.Should().Be(numberOfErrorsExpected);

        result.ShouldHaveValidationErrorFor(c => c.NumberOfAttendees)
            .WithErrorMessage(NumberOfAttendeesViewModelValidator.NumberOfAttendeesEmpty);
    }
}
