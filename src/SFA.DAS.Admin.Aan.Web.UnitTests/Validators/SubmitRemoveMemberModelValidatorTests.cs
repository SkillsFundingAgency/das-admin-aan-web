using FluentValidation.TestHelper;
using SFA.DAS.Admin.Aan.Application.Constants;
using SFA.DAS.Admin.Aan.Web.Models.RemoveMember;
using SFA.DAS.Admin.Aan.Web.Validators;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Validators;
public class SubmitRemoveMemberModelValidatorTests
{
    [Test]
    public void Validate_HasRemoveConfirmedIsTrue_ShouldNotHaveValidationForHasRemoveConfirmed()
    {
        // Arrange
        var model = new SubmitRemoveMemberModel()
        {
            HasRemoveConfirmed = true
        };

        // Act
        var sut = new SubmitRemoveMemberModelValidator();
        var result = sut.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.HasRemoveConfirmed);
    }

    [Test]
    public void Validate_HasRemoveConfirmedIsFalse_ShouldHaveValidationForHasRemoveConfirmed()
    {
        // Arrange
        var model = new SubmitRemoveMemberModel()
        {
            HasRemoveConfirmed = false
        };

        // Act
        var sut = new SubmitRemoveMemberModelValidator();
        var result = sut.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.HasRemoveConfirmed).WithErrorMessage(SubmitRemoveMemberModelValidator.HasRemoveConfirmedValidationMessage);
    }

    [Test]
    public void Validate_StatusIsSet_ShouldNotHaveValidationForStatus()
    {
        // Arrange
        var model = new SubmitRemoveMemberModel()
        {
            Status = MembershipStatusType.Deleted
        };

        // Act
        var sut = new SubmitRemoveMemberModelValidator();
        var result = sut.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Status);
    }

    [Test]
    public void Validate_StatusIsNotSet_ShouldHaveValidationForStatus()
    {
        // Arrange
        var model = new SubmitRemoveMemberModel();

        // Act
        var sut = new SubmitRemoveMemberModelValidator();
        var result = sut.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Status).WithErrorMessage(SubmitRemoveMemberModelValidator.ReasonForRemoveMemberRequired);
    }
}
