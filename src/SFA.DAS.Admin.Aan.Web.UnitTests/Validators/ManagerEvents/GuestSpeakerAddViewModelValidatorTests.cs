using FluentAssertions;
using FluentValidation.TestHelper;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;
using SFA.DAS.Admin.Aan.Web.Validators;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Validators.ManagerEvents;

internal class GuestSpeakerAddViewModelValidatorTests
{

    [Test]
    public void Validate_EmptySpeakerDetails_Invalid()
    {
        var model = new GuestSpeakerAddViewModel();

        var sut = new GuestSpeakerAddViewModelValidator();
        var result = sut.TestValidate(model);

        result.Errors.Count.Should().Be(2);
        result.ShouldHaveValidationErrorFor(c => c.JobRoleAndOrganisation)
            .WithErrorMessage(GuestSpeakerAddViewModelValidator.JobRoleAndOrganisationEmpty);

        result.ShouldHaveValidationErrorFor(c => c.Name)
            .WithErrorMessage(GuestSpeakerAddViewModelValidator.NameEmpty);
    }

    [TestCase(0, GuestSpeakerAddViewModelValidator.NameEmpty, false)]
    [TestCase(1, null, true)]
    [TestCase(GuestSpeakerAddViewModelValidator.GuestSpeakerMaximumLength, null, true)]
    [TestCase(GuestSpeakerAddViewModelValidator.GuestSpeakerMaximumLength + 1,
        GuestSpeakerAddViewModelValidator.NameTooLong, false)]
    public void Validate_GuestSpeakerName(int name, string? errorMessage, bool isValid)
    {
        var model = new GuestSpeakerAddViewModel
        { JobRoleAndOrganisation = "x", Name = new string('x', name) };

        var sut = new GuestSpeakerAddViewModelValidator();
        var result = sut.TestValidate(model);

        if (!isValid)
        {
            result.ShouldHaveValidationErrorFor(c => c.Name)
                .WithErrorMessage(errorMessage);
        }
        else
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    [TestCase("name 1", true, null)]
    [TestCase("name @", false, GuestSpeakerAddViewModelValidator.NameHasExcludedCharacter)]
    [TestCase("name #", false, GuestSpeakerAddViewModelValidator.NameHasExcludedCharacter)]
    [TestCase("name $", false, GuestSpeakerAddViewModelValidator.NameHasExcludedCharacter)]
    [TestCase("name ^", false, GuestSpeakerAddViewModelValidator.NameHasExcludedCharacter)]
    [TestCase("name =", false, GuestSpeakerAddViewModelValidator.NameHasExcludedCharacter)]
    [TestCase("name +", false, GuestSpeakerAddViewModelValidator.NameHasExcludedCharacter)]
    [TestCase("name \\", false, GuestSpeakerAddViewModelValidator.NameHasExcludedCharacter)]
    [TestCase("name /", false, GuestSpeakerAddViewModelValidator.NameHasExcludedCharacter)]
    [TestCase("name <", false, GuestSpeakerAddViewModelValidator.NameHasExcludedCharacter)]
    [TestCase("name >", false, GuestSpeakerAddViewModelValidator.NameHasExcludedCharacter)]
    public void Validate_GustSpeakerName_CheckInvalidCharacters(string? name, bool isValid, string? errorMessage)
    {
        var model = new GuestSpeakerAddViewModel { Name = name, JobRoleAndOrganisation = "role" };

        var sut = new GuestSpeakerAddViewModelValidator();
        var result = sut.TestValidate(model);

        if (!isValid)
        {
            result.ShouldHaveValidationErrorFor(c => c.Name)
                .WithErrorMessage(errorMessage);
        }
        else
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    [TestCase(0, GuestSpeakerAddViewModelValidator.JobRoleAndOrganisationEmpty, false)]
    [TestCase(1, null, true)]
    [TestCase(GuestSpeakerAddViewModelValidator.GuestSpeakerMaximumLength, null, true)]
    [TestCase(GuestSpeakerAddViewModelValidator.GuestSpeakerMaximumLength + 1,
       GuestSpeakerAddViewModelValidator.JobRoleAndOrganisationTooLong, false)]
    public void Validate_GuestRoleAndDescription(int jobRoleAndDescriptionLength, string? errorMessage, bool isValid)
    {
        var model = new GuestSpeakerAddViewModel
        { JobRoleAndOrganisation = new string('x', jobRoleAndDescriptionLength), Name = "name" };

        var sut = new GuestSpeakerAddViewModelValidator();
        var result = sut.TestValidate(model);

        if (!isValid)
        {
            result.ShouldHaveValidationErrorFor(c => c.JobRoleAndOrganisation)
                .WithErrorMessage(errorMessage);
        }
        else
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    [TestCase("jobRoleAndOrganisation1", true, null)]
    [TestCase("jobRoleAndOrganisation@", false, GuestSpeakerAddViewModelValidator.JobRoleAndOrganisationHasExcludedCharacter)]
    [TestCase("jobRoleAndOrganisation#", false, GuestSpeakerAddViewModelValidator.JobRoleAndOrganisationHasExcludedCharacter)]
    [TestCase("jobRoleAndOrganisation$", false, GuestSpeakerAddViewModelValidator.JobRoleAndOrganisationHasExcludedCharacter)]
    [TestCase("jobRoleAndOrganisation^", false, GuestSpeakerAddViewModelValidator.JobRoleAndOrganisationHasExcludedCharacter)]
    [TestCase("jobRoleAndOrganisation=", false, GuestSpeakerAddViewModelValidator.JobRoleAndOrganisationHasExcludedCharacter)]
    [TestCase("jobRoleAndOrganisation+", false, GuestSpeakerAddViewModelValidator.JobRoleAndOrganisationHasExcludedCharacter)]
    [TestCase("jobRoleAndOrganisation\\", false, GuestSpeakerAddViewModelValidator.JobRoleAndOrganisationHasExcludedCharacter)]
    [TestCase("jobRoleAndOrganisation/", false, GuestSpeakerAddViewModelValidator.JobRoleAndOrganisationHasExcludedCharacter)]
    [TestCase("jobRoleAndOrganisation<", false, GuestSpeakerAddViewModelValidator.JobRoleAndOrganisationHasExcludedCharacter)]
    [TestCase("jobRoleAndOrganisation>", false, GuestSpeakerAddViewModelValidator.JobRoleAndOrganisationHasExcludedCharacter)]
    public void Validate_GustSpeakerJobRoleAndDescription_CheckInvalidCharacters(string? jobRoleAndDescription, bool isValid, string? errorMessage)
    {
        var model = new GuestSpeakerAddViewModel { Name = "name", JobRoleAndOrganisation = jobRoleAndDescription };

        var sut = new GuestSpeakerAddViewModelValidator();
        var result = sut.TestValidate(model);

        if (!isValid)
        {
            result.ShouldHaveValidationErrorFor(c => c.JobRoleAndOrganisation)
                .WithErrorMessage(errorMessage);
        }
        else
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
