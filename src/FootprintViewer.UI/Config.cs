using FootprintViewer.AppStates;
using FootprintViewer.UI.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace FootprintViewer.UI;

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

    [JsonProperty(PropertyName = "MapBackgroundFiles")]
    public string[] MapBackgroundFiles { get; internal set; } = Array.Empty<string>();

    [JsonProperty(PropertyName = "PlannedScheduleState")]
    public PlannedScheduleState PlannedScheduleState { get; internal set; } = PlannedScheduleState.None;

    [JsonProperty(PropertyName = "LastPlannedScheduleConnection")]
    public DatabaseConnection? LastPlannedScheduleConnection { get; internal set; }

    [JsonProperty(PropertyName = "LastPlannedScheduleJsonFile")]
    public string? LastPlannedScheduleJsonFile { get; internal set; }

    [JsonProperty(PropertyName = "SelectedMapSnapshotExtension")]
    public string SelectedMapSnapshotExtension { get; internal set; } = "*.png";

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