using FluentAssertions;
using FluentValidation.TestHelper;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;
using SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Validators.ManageEvents;

public class NotifyAttendeesViewModelValidatorTests
{
    [TestCase(true)]
    [TestCase(false)]
    public void Validate_NumberOfAttendeesIsValid(bool notifyAttendees)
    {
        var model = new NotifyAttendeesViewModel { IsNotifyAttendees = notifyAttendees };
        var sut = new NotifyAttendeesViewModelValidator();
        var result = sut.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void Validate_NumberOfAttendees_NotSet()
    {
        var model = new NotifyAttendeesViewModel { IsNotifyAttendees = null };

        var sut = new NotifyAttendeesViewModelValidator();
        var result = sut.TestValidate(model);

        var numberOfErrorsExpected = 1;
        result.Errors.Count.Should().Be(numberOfErrorsExpected);

        result.ShouldHaveValidationErrorFor(c => c.IsNotifyAttendees)
            .WithErrorMessage(NotifyAttendeesViewModelValidator.NotifyAttendeesErrorMessage);
    }
}