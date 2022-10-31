using Newtonsoft.Json;

namespace DataSettingsSample.Models;

[JsonObject]
public class Satellite
{
    [JsonProperty("Name")]
    public string Name { get; set; } = "Default";

    [JsonProperty("ValueInt")]
    public int ValueInt { get; set; }
}
