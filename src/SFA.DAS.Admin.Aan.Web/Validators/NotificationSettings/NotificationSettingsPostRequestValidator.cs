﻿using FluentValidation;
using SFA.DAS.Admin.Aan.Web.Models.NotificationSettings;

namespace SFA.DAS.Admin.Aan.Web.Validators.NotificationSettings
{
    public class NotificationSettingsPostRequestValidator : AbstractValidator<NotificationSettingsPostRequest>
    {
        public const string ErrorMessage = "Select if you would like email updates about ambassadors signing up to your events";

        public NotificationSettingsPostRequestValidator()
        {
            RuleFor(x => x.ReceiveNotifications)
                .NotEmpty()
                .WithMessage(ErrorMessage);
        }
    }
}
