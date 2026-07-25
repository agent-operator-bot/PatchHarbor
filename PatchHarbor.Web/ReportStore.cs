using System.Text.Json;
using System.Text.Json.Serialization;
using PatchHarbor.Web.Models;

namespace PatchHarbor.Web;

public sealed class ReportStore
{
    private readonly string _path;
    private readonly string _auditPath;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public ReportStore(IWebHostEnvironment environment)
    {
        var dataDirectory = Path.Combine(environment.ContentRootPath, "data");
        Directory.CreateDirectory(dataDirectory);
        _path = Path.Combine(dataDirectory, "reports.json");
        _auditPath = Path.Combine(dataDirectory, "audit.json");
    }

    public async Task<IReadOnlyList<VulnerabilityReport>> ListAsync(CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken);
        try { return await ReadAsync(cancellationToken); }
        finally { _gate.Release(); }
    }

    public async Task<VulnerabilityReport> AddAsync(VulnerabilityReport report, CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            var reports = (await ReadAsync(cancellationToken)).ToList();
            reports.Add(report);
            await WriteAsync(reports, cancellationToken);
            await AppendAuditAsync(new AuditEvent(Guid.NewGuid(), report.Id, "ReportSubmitted", null, report.Status, DateTimeOffset.UtcNow), cancellationToken);
            return report;
        }
        finally { _gate.Release(); }
    }

    public async Task<VulnerabilityReport?> UpdateStatusAsync(Guid id, ReportStatus status, CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            var reports = (await ReadAsync(cancellationToken)).ToList();
            var index = reports.FindIndex(report => report.Id == id);
            if (index < 0) return null;
            var previousStatus = reports[index].Status;
            reports[index] = reports[index] with
            {
                Status = status,
                DisclosedAt = status == ReportStatus.Disclosed ? DateTimeOffset.UtcNow : reports[index].DisclosedAt
            };
            await WriteAsync(reports, cancellationToken);
            if (previousStatus != status)
                await AppendAuditAsync(new AuditEvent(Guid.NewGuid(), id, "StatusChanged", previousStatus, status, DateTimeOffset.UtcNow), cancellationToken);
            return reports[index];
        }
        finally { _gate.Release(); }
    }

    public async Task<IReadOnlyList<AuditEvent>> ListAuditAsync(Guid reportId, CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            if (!File.Exists(_auditPath)) return [];
            await using var stream = File.OpenRead(_auditPath);
            var events = await JsonSerializer.DeserializeAsync<List<AuditEvent>>(stream, _json, cancellationToken) ?? [];
            return events.Where(item => item.ReportId == reportId).OrderBy(item => item.OccurredAt).ToList();
        }
        finally { _gate.Release(); }
    }

    private async Task<IReadOnlyList<VulnerabilityReport>> ReadAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_path)) return [];
        await using var stream = File.OpenRead(_path);
        return await JsonSerializer.DeserializeAsync<List<VulnerabilityReport>>(stream, _json, cancellationToken) ?? [];
    }

    private async Task WriteAsync(IReadOnlyList<VulnerabilityReport> reports, CancellationToken cancellationToken)
    {
        await using var stream = File.Create(_path);
        await JsonSerializer.SerializeAsync(stream, reports, _json, cancellationToken);
    }

    private async Task AppendAuditAsync(AuditEvent auditEvent, CancellationToken cancellationToken)
    {
        var events = new List<AuditEvent>();
        if (File.Exists(_auditPath))
        {
            await using var input = File.OpenRead(_auditPath);
            events = await JsonSerializer.DeserializeAsync<List<AuditEvent>>(input, _json, cancellationToken) ?? [];
        }
        events.Add(auditEvent);
        await using var output = File.Create(_auditPath);
        await JsonSerializer.SerializeAsync(output, events, _json, cancellationToken);
    }
}
