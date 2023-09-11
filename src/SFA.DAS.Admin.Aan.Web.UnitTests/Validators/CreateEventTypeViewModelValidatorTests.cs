using FluentValidation.TestHelper;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;
using SFA.DAS.Admin.Aan.Web.Validators.CreateEvent;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Validators;
public class CreateEventTypeViewModelValidatorTests
{
    [TestCase(null, false, CreateEventTypeViewModelValidator.EventTitleEmpty)]
    [TestCase("title 1", true, null)]

    [TestCase("title @", false, CreateEventTypeViewModelValidator.EventTitleHasExcludedCharacter)]
    [TestCase("title #", false, CreateEventTypeViewModelValidator.EventTitleHasExcludedCharacter)]
    [TestCase("title $", false, CreateEventTypeViewModelValidator.EventTitleHasExcludedCharacter)]
    [TestCase("title ^", false, CreateEventTypeViewModelValidator.EventTitleHasExcludedCharacter)]
    [TestCase("title =", false, CreateEventTypeViewModelValidator.EventTitleHasExcludedCharacter)]
    [TestCase("title +", false, CreateEventTypeViewModelValidator.EventTitleHasExcludedCharacter)]
    [TestCase("title \\", false, CreateEventTypeViewModelValidator.EventTitleHasExcludedCharacter)]
    [TestCase("title /", false, CreateEventTypeViewModelValidator.EventTitleHasExcludedCharacter)]
    [TestCase("title <", false, CreateEventTypeViewModelValidator.EventTitleHasExcludedCharacter)]
    [TestCase("title >", false, CreateEventTypeViewModelValidator.EventTitleHasExcludedCharacter)]
    public void Validate_EmptyEventTitle_Check(string? title, bool isValid, string? errorMessage)
    {
        var model = new CreateEventTypeViewModel { EventTitle = title, EventRegionId = 1, EventTypeId = 2 };

        var sut = new CreateEventTypeViewModelValidator();
        var result = sut.TestValidate(model);

        if (!isValid)
        {
            result.ShouldHaveValidationErrorFor(c => c.EventTitle)
                .WithErrorMessage(errorMessage);
        }
        else
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    [TestCase(1, true)]
    [TestCase(null, false)]
    public void Validate_EventType_Check(int? eventTypeId, bool isValid)
    {
        var model = new CreateEventTypeViewModel { EventTitle = "title", EventRegionId = 1, EventTypeId = eventTypeId };

        var sut = new CreateEventTypeViewModelValidator();
        var result = sut.TestValidate(model);

        if (!isValid)
        {
            result.ShouldHaveValidationErrorFor(c => c.EventTypeId)
                .WithErrorMessage(CreateEventTypeViewModelValidator.EventTypeEmpty);
        }
        else
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    [TestCase(1, true)]
    [TestCase(null, false)]
    public void Validate_RegionId_Check(int? regionId, bool isValid)
    {
        var model = new CreateEventTypeViewModel { EventTitle = "title", EventRegionId = regionId, EventTypeId = 1 };

        var sut = new CreateEventTypeViewModelValidator();
        var result = sut.TestValidate(model);

        if (!isValid)
        {
            result.ShouldHaveValidationErrorFor(c => c.EventRegionId)
                .WithErrorMessage(CreateEventTypeViewModelValidator.EventRegionEmpty);
        }
        else
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
