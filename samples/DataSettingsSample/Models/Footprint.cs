﻿using Newtonsoft.Json;

namespace DataSettingsSample.Models;

[JsonObject]
public class Footprint
{
    [JsonProperty("Name")]
    public string Name { get; set; } = "Default";

    [JsonProperty("Value")]
    public double Value { get; set; }
}
