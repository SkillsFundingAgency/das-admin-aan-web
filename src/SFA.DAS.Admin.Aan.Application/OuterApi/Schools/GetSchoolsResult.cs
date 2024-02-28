namespace SFA.DAS.Admin.Aan.Application.OuterApi.Schools;

public class GetSchoolsResult
{
    public List<SchoolItem> Schools { get; set; } = [];
}

public class SchoolItem
{
    public string? Name { get; set; }
    public string? Urn { get; set; }
}
