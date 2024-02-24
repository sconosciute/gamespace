using System.Text.Json.Serialization;

namespace gamespace.Managers;

public class LaunchSettings
{
    [JsonPropertyName("DefaultResWidth")] public int DefaultResWidth { get; init; }
    [JsonPropertyName("DefaultResHeight")] public int DefaultResHeight { get; init; }
    [JsonPropertyName("IsFullScreen")] public bool IsFullScreened { get; init; }
    [JsonPropertyName("IsDynamic")] public bool IsDynamic { get; init; }
}