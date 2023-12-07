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
    public const string EventDetails = nameof(EventDetails);

    public static class CreateEvent
    {
        public const string EventFormat = nameof(EventFormat);
        public const string EventType = nameof(EventType);
        public const string Description = nameof(Description);
        public const string HasGuestSpeakers = nameof(HasGuestSpeakers);
        public const string GuestSpeakerList = nameof(GuestSpeakerList);
        public const string GuestSpeakerAdd = nameof(GuestSpeakerAdd);
        public const string GuestSpeakerDelete = nameof(GuestSpeakerDelete);
        public const string CreateNewEvent = nameof(CreateNewEvent);
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
        public const string UpdateEventType = nameof(UpdateEventType);
        public const string UpdateDateAndTime = nameof(UpdateDateAndTime);
        public const string UpdateDescription = nameof(UpdateDescription);
        public const string UpdateHasGuestSpeakers = nameof(UpdateHasGuestSpeakers);
        public const string UpdateGuestSpeakerList = nameof(UpdateGuestSpeakerList);
        public const string UpdateGuestSpeakerAdd = nameof(UpdateGuestSpeakerAdd);
        public const string UpdateGuestSpeakerDelete = nameof(UpdateGuestSpeakerDelete);
        public const string UpdateOrganiserDetails = nameof(UpdateOrganiserDetails);
        public const string UpdateIsAtSchool = nameof(UpdateIsAtSchool);
        public const string UpdateSchoolName = nameof(UpdateSchoolName);
        public const string UpdateNumberOfAttendees = nameof(UpdateNumberOfAttendees);
    }
}