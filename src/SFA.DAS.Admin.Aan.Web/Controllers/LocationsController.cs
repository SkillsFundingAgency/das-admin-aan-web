using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Application.Services;

namespace SFA.DAS.Admin.Aan.Web.Controllers;
[Authorize]
public class LocationsController : Controller
{
    private readonly IOuterApiClient _outerApiClient;

    public LocationsController(IOuterApiClient outerApiClient)
    {
        _outerApiClient = outerApiClient;
    }

    [HttpGet]
    [Route("/locations")]
    public async Task<IActionResult> GetAddresses([FromQuery] string query, CancellationToken cancellationToken)
    {
        var result = await _outerApiClient.GetAddresses(query, cancellationToken);

        return Ok(result.Addresses);
    }
}
