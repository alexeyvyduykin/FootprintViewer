using Newtonsoft.Json;

namespace JsonDataBuilderSample;

public static class JsonHelper
{
    public static void SerializeToFile(string path, object? value, JsonSerializerSettings? settings = null)
    {
        using StreamWriter file = File.CreateText(path);

        // file with GeoJSON
        var serializer = NetTopologySuite.IO.GeoJsonSerializer.CreateDefault(settings);

        serializer.CheckAdditionalContent = true;

        serializer.Serialize(file, value);
    }

    public static object? DeserializeFromFile<T>(string path, JsonSerializerSettings? settings = null)
    {
        using StreamReader file = File.OpenText(path);

        // file with GeoJSON
        var serializer = NetTopologySuite.IO.GeoJsonSerializer.CreateDefault(settings);

        return serializer.Deserialize(file, typeof(T));
    }

    public static bool Verified<T>(string path)
    {
        object? res;

        try
        {
            res = DeserializeFromFile<T>(path, new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Error
            });
        }
        catch (Exception)
        {
            return false;
        }

        return res is T;
    }
}
