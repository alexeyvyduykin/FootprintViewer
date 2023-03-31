using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FootprintViewer.Data.Models;

[JsonObject]
[JsonConverter(typeof(TaskResultConverter))]
public interface ITaskResult
{
    string TaskName { get; set; }

    string SatelliteName { get; set; }

    Interval Interval { get; set; }

    Interval? Transition { get; set; }
}

public class TaskResultConverter : JsonConverter
{
    public override bool CanWrite => false;

    public override bool CanRead => true;

    public override bool CanConvert(Type objectType) => objectType == typeof(ITaskResult);

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) => throw new InvalidOperationException("Use default serialization.");

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return null;
        }

        var jsonObject = JObject.Load(reader);

        ITaskResult? taskResult;

        if (jsonObject.ContainsKey(nameof(ObservationTaskResult.Geometry)))
        {
            taskResult = new ObservationTaskResult();
        }
        else if (jsonObject.ContainsKey(nameof(CommunicationTaskResult.Type)))
        {
            taskResult = new CommunicationTaskResult();
        }
        else
        {
            return null;
        }

        serializer.Populate(jsonObject.CreateReader(), taskResult!);

        return taskResult;
    }
}