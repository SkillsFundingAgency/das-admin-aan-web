using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Admin.Aan.Application.Constants;

[ExcludeFromCodeCoverage]
public static class ManageEventValidation
{
    public const int EventOutlineMaxLength = 200;
    public const int EventSummaryMaxLength = 2000;
    public const int EventOnlineLinkMaxLength = 1000;
    public const int GuestSpeakerMaximumLength = 200;
    public const int OrganiserNameMaximumLength = 200;
    public const int OrganiserEmailMaximumLength = 256;

}