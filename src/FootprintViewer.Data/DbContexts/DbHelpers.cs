using FootprintViewer.Data.Models;
using Newtonsoft.Json;

namespace FootprintViewer.Data.DbContexts;

public static class DbHelpers
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

    public static object? DeserializeFromFile<T>(string path, JsonSerializerSettings? settings = null)
    {
        using StreamReader file = File.OpenText(path);

        // file with GeoJSON
        var serializer = NetTopologySuite.IO.GeoJsonSerializer.CreateDefault(settings);

        return serializer.Deserialize(file, typeof(T));
    }

    public static bool TryValidateContext(DbKeys key, Func<DbCustomContext> creator)
    {
        using var context = creator.Invoke();
        return context.Valid(GetType(key));
    }

    public static bool IsKeyEquals(DbKeys? key, DbKeys? dbKeys)
    {
        return Equals(key, dbKeys);
    }

    public static bool IsKeyEquals(string? key, DbKeys? dbKeys)
    {
        var res = Enum.TryParse<DbKeys>(key, true, out var result);

        if (res == true)
        {
            return Equals(result, dbKeys);
        }

        return res;
    }

    public static TableInfoType GetTableType(string? key)
    {
        Enum.TryParse<DbKeys>(key, true, out var result);
        return result switch
        {
            DbKeys.UserGeometries => TableInfoType.UserGeometry,
            _ => throw new Exception($"Table info for key={key} not register."),
        };
    }

    public static Type GetType(DbKeys key)
    {
        return key switch
        {
            DbKeys.UserGeometries => typeof(UserGeometry),
            DbKeys.PlannedSchedules => typeof(PlannedScheduleResult),
            _ => throw new Exception(),
        };
    }

    public static Type GetType(string key)
    {
        Enum.TryParse<DbKeys>(key, true, out var result);
        return result switch
        {
            DbKeys.UserGeometries => typeof(UserGeometry),
            DbKeys.PlannedSchedules => typeof(PlannedScheduleResult),
            _ => throw new Exception(),
        };
    }

    public static bool JsonValidation<T>(string path)
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

    public static async Task<bool> JsonValidationAsync(DbKeys key, string path)
    {
        return await Task.Run(() =>
        {
            return key switch
            {
                DbKeys.UserGeometries => JsonValidation<List<UserGeometry>>(path),
                _ => throw new Exception($"DbHelper key={key} not register.")
            };
        });
    }

    private static IList<object> GetValues<T>(string path)
    {
        try
        {
            var res = DeserializeFromFile<IList<T>>(path, new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Error
            });

            if (res is IList<T> list)
            {
                return list.Cast<object>().ToList();
            }
        }
        catch (Exception)
        {
            return Array.Empty<object>();
        }

        return Array.Empty<object>();
    }
}
