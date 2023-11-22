using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Admin.Aan.Web.Infrastructure;

[ExcludeFromCodeCoverage]
public static class RouteNames
{
    public const string AdministratorHub = nameof(AdministratorHub);
    public const string NetworkEvents = nameof(NetworkEvents);
    public const string DeleteEvent = nameof(DeleteEvent);
    public const string DeleteEventConfirmation = nameof(DeleteEventConfirmation);
    public const string CalendarEvent = nameof(CalendarEvent);




    public static class ManageEvent
    {
        public const string EventFormat = nameof(EventFormat);
        public const string EventType = nameof(EventType);
        public const string Description = nameof(Description);
        public const string HasGuestSpeakers = nameof(HasGuestSpeakers);
        public const string GuestSpeakerList = nameof(GuestSpeakerList);
        public const string GuestSpeakerAdd = nameof(GuestSpeakerAdd);
        public const string GuestSpeakerDelete = nameof(GuestSpeakerDelete);
        public const string CreateEvent = nameof(CreateEvent);
        public const string DateAndTime = nameof(DateAndTime);
        public const string Location = nameof(Location);
        public const string IsAtSchool = nameof(IsAtSchool);
        public const string SchoolName = nameof(SchoolName);
        public const string OrganiserDetails = nameof(OrganiserDetails);
        public const string NumberOfAttendees = nameof(NumberOfAttendees);
        public const string CheckYourAnswers = nameof(CheckYourAnswers);
        public const string EventPublished = nameof(EventPublished);
        public const string PreviewEvent = nameof(PreviewEvent);
    }

    public static class UpdateEvent
    {
        public const string UpdatePreviewEvent = nameof(UpdatePreviewEvent);
        public const string UpdateEventFormat = nameof(UpdateEventFormat);
        public const string UpdateLocation = nameof(UpdateLocation);
    }
}