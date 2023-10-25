namespace SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

public class SchoolNameViewModel : ManageEventViewModelBase
{
    public string? SearchResult { get; set; }

    public string? SearchTerm { get; set; }
    public string? Name { get; set; }
    public string? Urn { get; set; }
    public bool DirectCallFromCheckYourAnswers { get; set; }
    public bool HasSeenPreview { get; set; }
}