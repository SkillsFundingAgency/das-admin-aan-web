using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Application.Services;

namespace SFA.DAS.Admin.Aan.Web.Controllers;

[Authorize]
public class SchoolsController : Controller
{
    private readonly IOuterApiClient _outerApiClient;

    public SchoolsController(IOuterApiClient outerApiClient)
    {
        _outerApiClient = outerApiClient;
    }

    [HttpGet]
    [Route("/schools")]
    public async Task<IActionResult> GetSchools([FromQuery] string query, CancellationToken cancellationToken)
    {
        var result = await _outerApiClient.GetSchools(query, cancellationToken);
        return Ok(result.Schools);
    }
}