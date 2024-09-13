using SFA.DAS.Admin.Aan.Application.OuterApi.NotificationSettings;

namespace SFA.DAS.Admin.Aan.Web.Models.NotificationSettings
{
    public class NotificationSettingsViewModel : NotificationSettingsPostRequest
    {
        public static implicit operator NotificationSettingsViewModel(GetNotificationSettingsResponse source)
        {
            return new NotificationSettingsViewModel
            {
                ReceiveNotifications = source.ReceiveNotifications
            };
        }
    }
}
