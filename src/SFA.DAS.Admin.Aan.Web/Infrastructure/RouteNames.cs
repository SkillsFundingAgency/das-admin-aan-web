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
        public const string EventGuestSpeaker = nameof(EventGuestSpeaker);
    }
}