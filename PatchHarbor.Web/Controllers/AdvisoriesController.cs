using Microsoft.AspNetCore.Mvc;
using PatchHarbor.Web.Models;

namespace PatchHarbor.Web.Controllers;

[ApiController]
[Route("api/advisories")]
public sealed class AdvisoriesController(ReportStore reports, CommunitySettingsStore community) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PublicAdvisory>>> List(CancellationToken cancellationToken)
    {
        var settings = await community.GetAsync(cancellationToken);
        if (!settings.PublicDisclosureEnabled) return NotFound();

        var disclosed = (await reports.ListAsync(cancellationToken))
            .Where(report => report.Status == ReportStatus.Disclosed && report.DisclosedAt.HasValue)
            .OrderByDescending(report => report.DisclosedAt)
            .Select(report => new PublicAdvisory(
                report.Id,
                report.Title,
                report.ServerName,
                report.Severity,
                report.AffectedComponent,
                report.AffectedVersion,
                report.DisclosedAt!.Value))
            .ToList();
        return Ok(disclosed);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PublicAdvisory>> Get(Guid id, CancellationToken cancellationToken)
    {
        var settings = await community.GetAsync(cancellationToken);
        if (!settings.PublicDisclosureEnabled) return NotFound();
        var report = (await reports.ListAsync(cancellationToken)).FirstOrDefault(item => item.Id == id && item.Status == ReportStatus.Disclosed && item.DisclosedAt.HasValue);
        return report is null
            ? NotFound()
            : Ok(new PublicAdvisory(report.Id, report.Title, report.ServerName, report.Severity, report.AffectedComponent, report.AffectedVersion, report.DisclosedAt!.Value));
    }
}
