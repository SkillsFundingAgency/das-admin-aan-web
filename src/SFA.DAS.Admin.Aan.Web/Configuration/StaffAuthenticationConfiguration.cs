using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Admin.Aan.Web.Configuration;

[ExcludeFromCodeCoverage]
public class StaffAuthenticationConfiguration
{
    public string WtRealm { get; set; } = null!;
    public string MetadataAddress { get; set; } = null!;
}
