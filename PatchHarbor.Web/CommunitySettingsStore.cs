using System.Text.Json;
using PatchHarbor.Web.Models;

namespace PatchHarbor.Web;

public sealed class CommunitySettingsStore
{
    private readonly string _path;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web) { WriteIndented = true };

    public CommunitySettingsStore(IWebHostEnvironment environment)
    {
        var dataDirectory = Path.Combine(environment.ContentRootPath, "data");
        Directory.CreateDirectory(dataDirectory);
        _path = Path.Combine(dataDirectory, "community.json");
    }

    public async Task<CommunitySettings> GetAsync(CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            if (!File.Exists(_path)) return new CommunitySettings();
            await using var stream = File.OpenRead(_path);
            return await JsonSerializer.DeserializeAsync<CommunitySettings>(stream, _json, cancellationToken) ?? new CommunitySettings();
        }
        finally { _gate.Release(); }
    }

    public async Task<CommunitySettings> SaveAsync(CommunitySettings settings, CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            await using var stream = File.Create(_path);
            await JsonSerializer.SerializeAsync(stream, settings, _json, cancellationToken);
            return settings;
        }
        finally { _gate.Release(); }
    }
}
