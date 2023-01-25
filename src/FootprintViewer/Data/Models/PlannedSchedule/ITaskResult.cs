using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace FootprintViewer.Data.Models;

[JsonObject]
[JsonConverter(typeof(TaskResultConverter))]
public interface ITaskResult
{
    string TaskName { get; set; }

    Interval Interval { get; set; }

    List<Interval>? Windows { get; set; }

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

        if (jsonObject.ContainsKey("Footprint"))
        {
            taskResult = new ObservationTaskResult();
        }
        else if (jsonObject.ContainsKey("Type"))
        {
            taskResult = new ComunicationTaskResult();
        }
        else
        {
            return null;
        }

        serializer.Populate(jsonObject.CreateReader(), taskResult!);

        return taskResult;
    }
}