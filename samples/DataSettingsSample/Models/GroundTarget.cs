using Newtonsoft.Json;

namespace FDataSettingsSample.Models;

[JsonObject]
public class GroundTarget
{
    [JsonProperty("Name")]
    public string Name { get; set; } = "Default";

    [JsonProperty("Value")]
    public double Value { get; set; }
}
