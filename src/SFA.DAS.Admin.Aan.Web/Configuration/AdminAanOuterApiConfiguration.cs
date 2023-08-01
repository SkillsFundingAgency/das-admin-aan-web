using System.Diagnostics.CodeAnalysis;
using SFA.DAS.Http.Configuration;

namespace SFA.DAS.Admin.Aan.Web.Configuration;

[ExcludeFromCodeCoverage]
public class AdminAanOuterApiConfiguration : IApimClientConfiguration
{
    public string ApiBaseUrl { get; set; } = null!;
    public string SubscriptionKey { get; set; } = null!;
    public string ApiVersion { get; set; } = null!;
}
