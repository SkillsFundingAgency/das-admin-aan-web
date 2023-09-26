using SFA.DAS.Admin.Aan.Web.Infrastructure;

namespace SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

public class EventAtSchoolViewModel : IEventPageEditFields
{
    public bool? IsAtSchool { get; set; }
    public string? PageTitle { get; set; }
    public string? PostLink { get; set; }
    public string? CancelLink { get; set; }
}