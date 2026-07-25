using Microsoft.AspNetCore.Mvc;
using PatchHarbor.Web.Models;

namespace PatchHarbor.Web.Controllers;

[ApiController]
[Route("api/community")]
public sealed class CommunityController(CommunitySettingsStore store, AccessPolicy access) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<CommunitySettings>> Get(CancellationToken cancellationToken)
        => Ok(await store.GetAsync(cancellationToken));

    [HttpPut]
    public async Task<ActionResult<CommunitySettings>> Update(UpdateCommunitySettingsRequest request, CancellationToken cancellationToken)
    {
        if (!access.IsAdministrator(Request)) return Unauthorized();
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Description))
            return BadRequest("Name and description are required.");
        if (request.Name.Length > 120 || request.Description.Length > 2_000 || request.SecurityContactUrl?.Length > 500)
            return BadRequest("One or more fields exceed the allowed length.");

        var settings = new CommunitySettings
        {
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            SecurityContactUrl = request.SecurityContactUrl?.Trim(),
            PublicDisclosureEnabled = request.PublicDisclosureEnabled
        };
        return Ok(await store.SaveAsync(settings, cancellationToken));
    }

}
