using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FootprintViewer.Data.Models;

[JsonObject]
[JsonConverter(typeof(TaskConverter))]
public interface ITask
{
    string Name { get; set; }
}

public class TaskConverter : JsonConverter
{
    public override bool CanWrite => false;

    public override bool CanRead => true;

    public override bool CanConvert(Type objectType) => objectType == typeof(ITask);

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) => throw new InvalidOperationException("Use default serialization.");

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return null;
        }

        var jsonObject = JObject.Load(reader);

        ITask? task;

        if (jsonObject.ContainsKey("GroundTargetName"))
        {
            task = new ObservationTask();
        }
        else if (jsonObject.ContainsKey("GroundStationName"))
        {
            task = new CommunicationTask();
        }
        else
        {
            return null;
        }

        serializer.Populate(jsonObject.CreateReader(), task!);

        return task;
    }
}