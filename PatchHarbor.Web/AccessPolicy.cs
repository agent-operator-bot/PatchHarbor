namespace PatchHarbor.Web;

public enum AccessRole
{
    None,
    Moderator,
    Administrator
}

public sealed class AccessPolicy(IConfiguration configuration)
{
    public AccessRole GetRole(HttpRequest request)
    {
        if (!request.Headers.TryGetValue("X-Admin-Key", out var supplied)) return AccessRole.None;
        var key = supplied.ToString();
        var adminKey = configuration["PatchHarbor:AdminKey"] ?? Environment.GetEnvironmentVariable("PATCHHARBOR_ADMIN_KEY");
        var moderatorKey = configuration["PatchHarbor:ModeratorKey"] ?? Environment.GetEnvironmentVariable("PATCHHARBOR_MODERATOR_KEY");
        if (!string.IsNullOrWhiteSpace(adminKey) && CryptographicEquals(key, adminKey)) return AccessRole.Administrator;
        if (!string.IsNullOrWhiteSpace(moderatorKey) && CryptographicEquals(key, moderatorKey)) return AccessRole.Moderator;
        return AccessRole.None;
    }

    public bool IsModerator(HttpRequest request) => GetRole(request) is AccessRole.Moderator or AccessRole.Administrator;
    public bool IsAdministrator(HttpRequest request) => GetRole(request) == AccessRole.Administrator;

    private static bool CryptographicEquals(string left, string right)
    {
        var leftBytes = System.Text.Encoding.UTF8.GetBytes(left);
        var rightBytes = System.Text.Encoding.UTF8.GetBytes(right);
        return leftBytes.Length == rightBytes.Length
            && System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(leftBytes, rightBytes);
    }
}
