using FluentValidation.TestHelper;
using SFA.DAS.Admin.Aan.Application.Constants;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;
using SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Validators.ManagerEvents;
public class EventFormatViewModelValidatorTests
{
    [Test]
    public void Validate_EmptyEventFormat_Invalid()
    {
        var model = new EventFormatViewModel();


        var sut = new EventFormatViewModelValidator();
        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.EventFormat)
            .WithErrorMessage(EventFormatViewModelValidator.EventFormatErrorMessage);
    }

    [TestCase(EventFormat.Hybrid)]
    [TestCase(EventFormat.InPerson)]
    [TestCase(EventFormat.Online)]
    public void Validate_EventFormat_Valid(EventFormat eventFormat)
    {
        var model = new EventFormatViewModel { EventFormat = eventFormat };


        var sut = new EventFormatViewModelValidator();
        var result = sut.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
