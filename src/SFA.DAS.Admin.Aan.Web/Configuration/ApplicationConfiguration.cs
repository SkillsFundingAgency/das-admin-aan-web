using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Admin.Aan.Web.Configuration;

[ExcludeFromCodeCoverage]
public class ApplicationConfiguration
{
    public string RedisConnectionString { get; set; } = null!;
    public string EmployerAanUrl { get; set; } = null!;
    public string ApprenticeAanUrl { get; set; } = null!;
    /// <summary>
    /// Gets or Sets UseDfESignIn Config Value.
    /// </summary>
    public bool UseDfESignIn { get; set; }
}
