using FluentValidation.TestHelper;
using SFA.DAS.Admin.Aan.Web.Models.ManageEvent;
using SFA.DAS.Admin.Aan.Web.Validators.ManageEvent;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Validators.ManagerEvents;
public class TypeViewModelValidatorTests
{
    [TestCase(null, false, TypeViewModelValidator.EventTitleEmpty)]
    [TestCase("title 1", true, null)]

    [TestCase("title @", false, TypeViewModelValidator.EventTitleHasExcludedCharacter)]
    [TestCase("title #", false, TypeViewModelValidator.EventTitleHasExcludedCharacter)]
    [TestCase("title $", false, TypeViewModelValidator.EventTitleHasExcludedCharacter)]
    [TestCase("title ^", false, TypeViewModelValidator.EventTitleHasExcludedCharacter)]
    [TestCase("title =", false, TypeViewModelValidator.EventTitleHasExcludedCharacter)]
    [TestCase("title +", false, TypeViewModelValidator.EventTitleHasExcludedCharacter)]
    [TestCase("title \\", false, TypeViewModelValidator.EventTitleHasExcludedCharacter)]
    [TestCase("title /", false, TypeViewModelValidator.EventTitleHasExcludedCharacter)]
    [TestCase("title <", false, TypeViewModelValidator.EventTitleHasExcludedCharacter)]
    [TestCase("title >", false, TypeViewModelValidator.EventTitleHasExcludedCharacter)]
    public void Validate_EmptyEventTitle_Check(string? title, bool isValid, string? errorMessage)
    {
        var model = new TypeViewModel { EventTitle = title, EventRegionId = 1, EventTypeId = 2 };

        var sut = new TypeViewModelValidator();
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
        var model = new TypeViewModel { EventTitle = "title", EventRegionId = 1, EventTypeId = eventTypeId };

        var sut = new TypeViewModelValidator();
        var result = sut.TestValidate(model);

        if (!isValid)
        {
            result.ShouldHaveValidationErrorFor(c => c.EventTypeId)
                .WithErrorMessage(TypeViewModelValidator.EventTypeEmpty);
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
        var model = new TypeViewModel { EventTitle = "title", EventRegionId = regionId, EventTypeId = 1 };

        var sut = new TypeViewModelValidator();
        var result = sut.TestValidate(model);

        if (!isValid)
        {
            result.ShouldHaveValidationErrorFor(c => c.EventRegionId)
                .WithErrorMessage(TypeViewModelValidator.EventRegionEmpty);
        }
        else
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
