using FluentValidation.TestHelper;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;
using SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Validators.ManageEvents;
public class EventTypeViewModelValidatorTests
{
    [TestCase(null, false, EventTypeViewModelValidator.EventTitleEmpty)]
    [TestCase("title 1", true, null)]

    [TestCase("title @", false, EventTypeViewModelValidator.EventTitleHasExcludedCharacter)]
    [TestCase("title #", false, EventTypeViewModelValidator.EventTitleHasExcludedCharacter)]
    [TestCase("title $", false, EventTypeViewModelValidator.EventTitleHasExcludedCharacter)]
    [TestCase("title ^", false, EventTypeViewModelValidator.EventTitleHasExcludedCharacter)]
    [TestCase("title =", false, EventTypeViewModelValidator.EventTitleHasExcludedCharacter)]
    [TestCase("title +", false, EventTypeViewModelValidator.EventTitleHasExcludedCharacter)]
    [TestCase("title \\", false, EventTypeViewModelValidator.EventTitleHasExcludedCharacter)]
    [TestCase("title /", false, EventTypeViewModelValidator.EventTitleHasExcludedCharacter)]
    [TestCase("title <", false, EventTypeViewModelValidator.EventTitleHasExcludedCharacter)]
    [TestCase("title >", false, EventTypeViewModelValidator.EventTitleHasExcludedCharacter)]
    public void Validate_EmptyEventTitle_Check(string? title, bool isValid, string? errorMessage)
    {
        var model = new EventTypeViewModel { EventTitle = title, EventRegionId = 1, EventTypeId = 2 };

        var sut = new EventTypeViewModelValidator();
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
        var model = new EventTypeViewModel { EventTitle = "title", EventRegionId = 1, EventTypeId = eventTypeId };

        var sut = new EventTypeViewModelValidator();
        var result = sut.TestValidate(model);

        if (!isValid)
        {
            result.ShouldHaveValidationErrorFor(c => c.EventTypeId)
                .WithErrorMessage(EventTypeViewModelValidator.EventTypeEmpty);
        }
        else
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    [TestCase(1, true)]
    [TestCase(0, true)]
    [TestCase(null, false)]
    public void Validate_RegionId_Check(int? regionId, bool isValid)
    {
        var model = new EventTypeViewModel { EventTitle = "title", EventRegionId = regionId, EventTypeId = 1 };

        var sut = new EventTypeViewModelValidator();
        var result = sut.TestValidate(model);

        if (!isValid)
        {
            result.ShouldHaveValidationErrorFor(c => c.EventRegionId)
                .WithErrorMessage(EventTypeViewModelValidator.EventRegionEmpty);
        }
        else
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
