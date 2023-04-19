using FootprintViewer.AppStates;
using FootprintViewer.Localization;
using Newtonsoft.Json;

namespace FootprintViewer.Fluent;

[JsonObject]
public class Config : ConfigBase
{
    /// <summary>
    /// Constructor for config population using Newtonsoft.JSON.
    /// </summary>
    public Config() : base()
    {

    }

    public Config(string filePath) : base(filePath)
    {

    }

    [JsonProperty(PropertyName = "FootprintSourceBuilders")]
    public string[] FootprintSourceBuilders { get; internal set; } = { "random", "database" };

    [JsonProperty(PropertyName = "GroundTargetSourceBuilders")]
    public string[] GroundTargetSourceBuilders { get; internal set; } = { "random", "database" };

    [JsonProperty(PropertyName = "GroundStationSourceBuilders")]
    public string[] GroundStationSourceBuilders { get; internal set; } = { "random", "database" };

    [JsonProperty(PropertyName = "SatelliteSourceBuilders")]
    public string[] SatelliteSourceBuilders { get; internal set; } = { "random", "database" };

    [JsonProperty(PropertyName = "UserGeometrySourceBuilders")]
    public string[] UserGeometrySourceBuilders { get; internal set; } = { "database" };

    [JsonProperty(PropertyName = "FootprintPreviewGeometrySourceBuilders")]
    public string[] FootprintPreviewGeometrySourceBuilders { get; internal set; } = { "file" };

    [JsonProperty(PropertyName = "MapBackgroundSourceBuilders")]
    public string[] MapBackgroundSourceBuilders { get; internal set; } = { "folder" };

    [JsonProperty(PropertyName = "FootprintPreviewSourceBuilders")]
    public string[] FootprintPreviewSourceBuilders { get; internal set; } = { "folder" };

    [JsonProperty(PropertyName = "LastLanguage")]
    public LanguageModel? LastLanguage { get; internal set; }

    [JsonProperty(PropertyName = "AvailableLocales")]
    public string[] AvailableLocales { get; internal set; } = { "en", "ru" };

    [JsonProperty(PropertyName = "LastOpenDirectory")]
    public string? LastOpenDirectory { get; internal set; }

    [JsonProperty(PropertyName = "LastOpenDatabase")]
    public DatabaseConnection? LastOpenDatabase { get; internal set; }

    [JsonProperty(PropertyName = "")]
    public string[] MapBackgroundFiles { get; internal set; } = Array.Empty<string>();

    //[DataMember]
    //private DataState<ISourceState> DataState { get; set; } = new();
}

[JsonObject]
public class DatabaseConnection
{
    [JsonProperty(PropertyName = "Host")]
    public string? Host { get; set; }

    [JsonProperty(PropertyName = "Database")]
    public string? Database { get; set; }

    [JsonProperty(PropertyName = "Username")]
    public string? Username { get; set; }

    [JsonProperty(PropertyName = "Password")]
    public string? Password { get; set; }
}
