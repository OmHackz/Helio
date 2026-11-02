using System.Text.Json.Serialization;

namespace Helio.Models;

public class AuthTokens
{
    /// <summary>Authentication provider: "microsoft" (default) or "elyby".</summary>
    public string Provider { get; set; } = "microsoft";

    /// <summary>
    /// How the Ely.by account was authenticated: "oauth" (device code) or
    /// "password" (Yggdrasil). Empty for Microsoft accounts.
    /// </summary>
    public string AuthMethod { get; set; } = string.Empty;

    public string MicrosoftRefreshToken { get; set; } = string.Empty;

    /// <summary>OAuth refresh token (Ely.by OAuth accounts only).</summary>
    public string OAuthRefreshToken { get; set; } = string.Empty;

    public string MinecraftAccessToken  { get; set; } = string.Empty;
    public DateTime MinecraftTokenExpiry { get; set; }
    public string PlayerUuid { get; set; } = string.Empty;
    public string PlayerName { get; set; } = string.Empty;

    /// <summary>Yggdrasil client token (Ely.by password accounts only).</summary>
    public string ClientToken { get; set; } = string.Empty;

    [JsonIgnore]
    public bool IsElyBy =>
        string.Equals(Provider, "elyby", StringComparison.OrdinalIgnoreCase);

    [JsonIgnore]
    public bool IsElyByOAuth =>
        IsElyBy && string.Equals(AuthMethod, "oauth", StringComparison.OrdinalIgnoreCase);

    [JsonIgnore]
    public bool IsMinecraftTokenValid =>
        !string.IsNullOrEmpty(MinecraftAccessToken) &&
        DateTime.UtcNow < MinecraftTokenExpiry;
}
