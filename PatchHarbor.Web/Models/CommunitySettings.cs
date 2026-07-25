namespace PatchHarbor.Web.Models;

public sealed record CommunitySettings
{
    public string Name { get; init; } = "PatchHarbor Community";
    public string Description { get; init; } = "A game-server community powered by PatchHarbor.";
    public string? SecurityContactUrl { get; init; }
    public bool PublicDisclosureEnabled { get; init; }
}

public sealed record UpdateCommunitySettingsRequest(
    string Name,
    string Description,
    string? SecurityContactUrl,
    bool PublicDisclosureEnabled);
