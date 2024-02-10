using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace gamespace.Managers;

public class launchSettings
{
    //[JsonProperty("DefaultResWidth")]
    [JsonPropertyName("DefaultResWidth")]
    public int DefaultResWidth { get; set; }
    [JsonPropertyName("DefaultResHeight")]
    public int DefaultResHeight { get; set; }
    [JsonPropertyName("IsFullScreen")]
    public bool IsFullScreened { get; set; }
    [JsonPropertyName("IsDynamic")]
    public bool IsDynamic { get; set;}
}