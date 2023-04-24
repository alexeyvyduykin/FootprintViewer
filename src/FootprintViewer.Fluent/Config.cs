using FootprintViewer.AppStates;
using FootprintViewer.Fluent.Models;
using FootprintViewer.Localization;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

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

    [JsonProperty(PropertyName = "MapBackgroundFiles")]
    public string[] MapBackgroundFiles { get; internal set; } = Array.Empty<string>();

    [JsonProperty(PropertyName = "PlannedScheduleState")]
    public PlannedScheduleState PlannedScheduleState { get; internal set; } = PlannedScheduleState.None;

    [JsonProperty(PropertyName = "LastPlannedScheduleConnection")]
    public DatabaseConnection? LastPlannedScheduleConnection { get; internal set; }

    [JsonProperty(PropertyName = "LastPlannedScheduleJsonFile")]
    public string? LastPlannedScheduleJsonFile { get; internal set; }

    public void ValidatePaths()
    {
        bool isAllExists = true;

        List<string> list = new();

        foreach (var item in MapBackgroundFiles)
        {
            if (File.Exists(item) == false)
            {
                isAllExists = false;
                continue;
            }

            list.Add(item);
        }

        if (isAllExists == false)
        {
            MapBackgroundFiles = list.ToArray();

            ToFile();
        }
    }
}