using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FootprintViewer.Helpers;

public static class JsonHelpers
{
    public static List<T> DeserializeFromStream<T>(string path)
    {
        var jsonString = File.ReadAllText(path);

        try
        {
            return JsonConvert.DeserializeObject<List<T>>(jsonString) ?? new List<T>();
        }
        catch (System.Exception)
        {
            var res = JsonConvert.DeserializeObject<T>(jsonString);

            if (res == null)
            {
                return new List<T>();
            }

            return new List<T>() { res };
        }
    }

    public static List<T> DeserializeFromStream<T>(Stream stream)
    {
        using var file = new StreamReader(stream);

        string jsonString = file.ReadToEnd();

        try
        {
            return JsonConvert.DeserializeObject<List<T>>(jsonString) ?? new List<T>();
        }
        catch (Exception)
        {
            var res = JsonConvert.DeserializeObject<T>(jsonString);

            if (res == null)
            {
                return new List<T>();
            }

            return new List<T>() { res };
        }
    }

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

    public static async Task<bool> VerifiedAsync<T>(string path)
    {
        return await Task.Run(() => Verified<T>(path));
    }
}
