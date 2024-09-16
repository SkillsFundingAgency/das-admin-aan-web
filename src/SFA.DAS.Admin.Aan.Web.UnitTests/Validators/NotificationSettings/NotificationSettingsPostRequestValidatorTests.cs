using FluentValidation.TestHelper;
using SFA.DAS.Admin.Aan.Web.Models.NotificationSettings;
using SFA.DAS.Admin.Aan.Web.Validators.NotificationSettings;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Validators.NotificationSettings
{
    public class NotificationSettingsPostRequestValidatorTests
    {
        [TestCase(true, true)]
        [TestCase(false, true)]
        [TestCase(null, false)]
        public void Validate_ReceiveNotifications(bool? receiveNotifications, bool isValid)
        {
            var model = new NotificationSettingsPostRequest() { ReceiveNotifications = receiveNotifications};

            var sut = new NotificationSettingsPostRequestValidator();
            var result = sut.TestValidate(model);

            if (!isValid)
            {
                result.ShouldHaveValidationErrorFor(c => c.ReceiveNotifications)
                    .WithErrorMessage(NotificationSettingsPostRequestValidator.ErrorMessage);
            }
            else
            {
                result.ShouldNotHaveAnyValidationErrors();
            }
        }
    }
}
