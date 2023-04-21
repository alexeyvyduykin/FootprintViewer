using Newtonsoft.Json;

namespace FootprintViewer.Fluent.Models;

[JsonObject]
public class DatabaseConnection
{
    [JsonProperty(PropertyName = "ConnectionString")]
    public required string ConnectionString { get; set; }

    [JsonProperty(PropertyName = "TableName")]
    public required string TableName { get; set; }

    [JsonProperty(PropertyName = "RecordName")]
    public required string RecordName { get; set; }
}
