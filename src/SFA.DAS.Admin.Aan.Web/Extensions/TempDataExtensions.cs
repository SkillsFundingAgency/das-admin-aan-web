using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace SFA.DAS.Admin.Aan.Web.Extensions
{
    public static class TempDataDictionaryExtensions
    {
        public static readonly string FlashMessageTitleTempDataKey = "FlashMessageTitle";
        public static readonly string FlashMessageBodyTempDataKey = "FlashMessageBody";
        public static readonly string FlashMessageTempDetailKey = "FlashMessageDetail";
        public static readonly string FlashMessageLevelTempDataKey = "FlashMessageLevel";

        public enum FlashMessageLevel
        {
            Info,
            Warning,
            Success
        }

        public static void AddFlashMessage(this ITempDataDictionary tempData, string title, FlashMessageLevel level)
        {
            tempData[FlashMessageBodyTempDataKey] = null;
            tempData[FlashMessageTitleTempDataKey] = title;
            tempData[FlashMessageLevelTempDataKey] = level;
        }

        public static void AddFlashMessage(this ITempDataDictionary tempData, string title, string body, FlashMessageLevel level)
        {
            tempData[FlashMessageBodyTempDataKey] = body;
            tempData[FlashMessageTitleTempDataKey] = title;
            tempData[FlashMessageLevelTempDataKey] = level;
        }
    }
}
