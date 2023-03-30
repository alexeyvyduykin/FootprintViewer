using Newtonsoft.Json;

namespace FootprintViewer.Data.Models;

[JsonObject]
public class Satellite
{
    [JsonProperty("Name")]
    public string Name { get; set; } = null!;

    [JsonProperty("Semiaxis")]
    public double Semiaxis { get; set; }

    [JsonProperty("Eccentricity")]
    public double Eccentricity { get; set; }

    [JsonProperty("InclinationDeg")]
    public double InclinationDeg { get; set; }

    [JsonProperty("ArgumentOfPerigeeDeg")]
    public double ArgumentOfPerigeeDeg { get; set; }

    [JsonProperty("LongitudeAscendingNodeDeg")]
    public double LongitudeAscendingNodeDeg { get; set; }

    [JsonProperty("RightAscensionAscendingNodeDeg")]
    public double RightAscensionAscendingNodeDeg { get; set; }

    [JsonProperty("Period")]
    public double Period { get; set; }

    [JsonProperty("Epoch")]
    public DateTime Epoch { get; set; }

    [JsonProperty("LookAngleDeg")]
    public double LookAngleDeg { get; set; }

    [JsonProperty("RadarAngleDeg")]
    public double RadarAngleDeg { get; set; }
}
