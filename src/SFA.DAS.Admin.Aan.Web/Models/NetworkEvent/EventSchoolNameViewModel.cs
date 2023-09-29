using SFA.DAS.Admin.Aan.Web.Infrastructure;

namespace SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

public class EventSchoolNameViewModel : EventSchoolNameDetailsModel, IEventPageEditFields
{
    public string? PageTitle { get; set; }
    public string? PostLink { get; set; }
    public string? CancelLink { get; set; }
    public string? SearchResult { get; set; }
}

public class EventSchoolNameDetailsModel
{
    public string? SearchTerm { get; set; }
    public string? Name { get; set; }
    public string? Urn { get; set; }
}