using FluentValidation.TestHelper;
using SFA.DAS.Admin.Aan.Application.Constants;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;
using SFA.DAS.Admin.Aan.Web.Validators.CreateEvent;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Validators;
public class CreateEventFormatViewModelValidatorTests
{
    [Test]
    public void Validate_EmptyEventFormat_Invalid()
    {
        var model = new CreateEventFormatViewModel();


        var sut = new CreateEventFormatViewModelValidator();
        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.EventFormat)
            .WithErrorMessage(CreateEventFormatViewModelValidator.EventFormatErrorMessage);
    }

    [TestCase(EventFormat.Hybrid)]
    [TestCase(EventFormat.InPerson)]
    [TestCase(EventFormat.Online)]
    public void Validate_EventFormat_Valid(EventFormat eventFormat)
    {
        var model = new CreateEventFormatViewModel { EventFormat = eventFormat };


        var sut = new CreateEventFormatViewModelValidator();
        var result = sut.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
