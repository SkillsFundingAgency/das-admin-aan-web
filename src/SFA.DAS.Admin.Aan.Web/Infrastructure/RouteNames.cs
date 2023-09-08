using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Admin.Aan.Web.Infrastructure;

[ExcludeFromCodeCoverage]
public static class RouteNames
{
    public const string AdministratorHub = nameof(AdministratorHub);
    public const string NetworkEvents = nameof(NetworkEvents);

    public static class CreateEvent
    {
        public const string EventFormat = nameof(EventFormat);
        public const string EventType = nameof(EventType);
        public const string EventDescription = nameof(EventDescription);
        public const string EventHasGuestSpeakers = nameof(EventHasGuestSpeakers);
        public const string GuestSpeakerList = nameof(GuestSpeakerList);
        public const string GuestSpeakerAdd = nameof(GuestSpeakerAdd);
        public const string GuestSpeakerDelete = nameof(GuestSpeakerDelete);
    }
}