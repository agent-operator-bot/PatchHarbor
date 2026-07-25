using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PatchHarbor.Web.Models;

namespace PatchHarbor.Web.Controllers;

[ApiController]
[Route("api/reports")]
public sealed class ReportsController(ReportStore store, AccessPolicy access) : ControllerBase
{
    [HttpPost]
    [EnableRateLimiting("public-submit")]
    public async Task<ActionResult<ReportReceipt>> Submit(CreateReportRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Description) || string.IsNullOrWhiteSpace(request.ServerName))
            return BadRequest("Title, description, and serverName are required.");

        if (request.Title.Length > 160 || request.Description.Length > 20_000 || request.ServerName.Length > 160 || request.ReproductionSteps?.Length > 20_000)
            return BadRequest("One or more fields exceed the allowed length.");

        var report = new VulnerabilityReport
        {
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            ServerName = request.ServerName.Trim(),
            Severity = request.Severity,
            AffectedComponent = request.AffectedComponent?.Trim(),
            AffectedVersion = request.AffectedVersion?.Trim(),
            ReproductionSteps = request.ReproductionSteps?.Trim(),
            ReporterContact = request.ReporterContact?.Trim()
        };

        await store.AddAsync(report, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = report.Id }, new ReportReceipt(report.Id, report.SubmittedAt, "Your report was received."));
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<VulnerabilityReport>>> List([FromQuery] ReportStatus? status, [FromQuery] Severity? severity, [FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 25, CancellationToken cancellationToken = default)
    {
        if (!access.IsModerator(Request)) return Unauthorized();
        page = Math.Clamp(page, 1, 10_000);
        pageSize = Math.Clamp(pageSize, 1, 100);
        var reports = await store.ListAsync(cancellationToken);
        var filtered = reports.Where(report =>
                (!status.HasValue || report.Status == status.Value)
                && (!severity.HasValue || report.Severity == severity.Value)
                && (string.IsNullOrWhiteSpace(q) || report.Title.Contains(q, StringComparison.OrdinalIgnoreCase) || report.ServerName.Contains(q, StringComparison.OrdinalIgnoreCase)))
            .OrderByDescending(report => report.SubmittedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        return Ok(filtered);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<VulnerabilityReport>> Get(Guid id, CancellationToken cancellationToken)
    {
        if (!access.IsModerator(Request)) return Unauthorized();
        var report = (await store.ListAsync(cancellationToken)).FirstOrDefault(item => item.Id == id);
        return report is null ? NotFound() : Ok(report);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<VulnerabilityReport>> UpdateStatus(Guid id, UpdateReportStatusRequest request, CancellationToken cancellationToken)
    {
        if (!access.IsModerator(Request)) return Unauthorized();
        var report = await store.UpdateStatusAsync(id, request.Status, cancellationToken);
        return report is null ? NotFound() : Ok(report);
    }

    [HttpGet("{id:guid}/audit")]
    public async Task<ActionResult<IReadOnlyList<AuditEvent>>> Audit(Guid id, CancellationToken cancellationToken)
    {
        if (!access.IsModerator(Request)) return Unauthorized();
        var report = (await store.ListAsync(cancellationToken)).FirstOrDefault(item => item.Id == id);
        return report is null ? NotFound() : Ok(await store.ListAuditAsync(id, cancellationToken));
    }

}
