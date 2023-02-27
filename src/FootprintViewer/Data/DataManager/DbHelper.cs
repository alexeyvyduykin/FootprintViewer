using FootprintViewer.Data.Models;
using FootprintViewer.ViewModels.Settings;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FootprintViewer.Data.DataManager;

internal static class DbHelper
{
    public static string ToConnectionString(string host, int port, string database, string username, string password)
    {
        return $"Host={host};Port={port};Database={database};Username={username};Password={password}";
    }

    public static (string host, int port, string database, string username, string password) FromConnectionString(string connectionString)
    {
        var pattern = @"Host=(?<host>[^@]+);Port=(?<port>[\d]+);Database=(?<database>[^@]+);Username=(?<username>[^@]+);Password=(?<password>[^@]+)";

        var matches = Regex.Matches(connectionString, pattern);

        var host = matches.FirstOrDefault()?.Groups["host"].Value ?? string.Empty;
        var port = matches.FirstOrDefault()?.Groups["port"].Value ?? string.Empty;
        var database = matches.FirstOrDefault()?.Groups["database"].Value ?? string.Empty;
        var username = matches.FirstOrDefault()?.Groups["username"].Value ?? string.Empty;
        var password = matches.FirstOrDefault()?.Groups["password"].Value ?? string.Empty;

        return (host, int.Parse(port ?? string.Empty), database, username, password);
    }

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

    public static Func<string, DbCustomContext> CreateDatabaseSource(string key, string tableName)
    {
        Enum.TryParse<DbKeys>(key, true, out var result);
        return result switch
        {
            DbKeys.Footprints => s => new FootprintDbContext(tableName, s),
            DbKeys.GroundTargets => s => new GroundTargetDbContext(tableName, s),
            DbKeys.Satellites => s => new SatelliteDbContext(tableName, s),
            DbKeys.GroundStations => s => new GroundStationDbContext(tableName, s),
            DbKeys.UserGeometries => s => new UserGeometryDbContext(tableName, s),
            DbKeys.PlannedSchedules => s => new PlannedScheduleDbContext(tableName, s),
            _ => throw new Exception($"DBContext for key={key} not register."),
        };
    }

    public static async Task EditAsync(string key, string connectionString, string tableName, string id, object newValue)
    {
        Enum.TryParse<DbKeys>(key, true, out var result);

        switch (result)
        {
            case DbKeys.UserGeometries:
                if (newValue is UserGeometry newUserGeometry)
                {
                    using var context = new UserGeometryDbContext(tableName, connectionString);
                    var userGeometry = await context.UserGeometries
                        .Where(b => b.Name == id)
                        .FirstOrDefaultAsync();

                    if (userGeometry != null)
                    {
                        userGeometry.Geometry = newUserGeometry.Geometry;

                        await context.SaveChangesAsync();
                    }
                }
                break;
            default:
                break;
        }
    }

    public static bool TryValidateContext(string key, string connectionString, string tableName)
    {
        using var context = CreateDatabaseSource(key, tableName).Invoke(connectionString);
        return context.Valid(GetType(key));
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
            DbKeys.Footprints => TableInfoType.Footprint,
            DbKeys.GroundTargets => TableInfoType.GroundTarget,
            DbKeys.Satellites => TableInfoType.Satellite,
            DbKeys.GroundStations => TableInfoType.GroundStation,
            DbKeys.UserGeometries => TableInfoType.UserGeometry,
            _ => throw new Exception($"Table info for key={key} not register."),
        };
    }

    public static Type GetType(string key)
    {
        Enum.TryParse<DbKeys>(key, true, out var result);
        return result switch
        {
            DbKeys.Footprints => typeof(Footprint),
            DbKeys.GroundTargets => typeof(GroundTarget),
            DbKeys.Satellites => typeof(Satellite),
            DbKeys.GroundStations => typeof(GroundStation),
            DbKeys.UserGeometries => typeof(UserGeometry),
            DbKeys.PlannedSchedules => typeof(PlannedScheduleResult),
            _ => throw new Exception(),
        };
    }

    public static Func<IList<string>, IList<object>> JsonReaderFromPaths(string key)
    {
        Enum.TryParse<DbKeys>(key, true, out var result);
        return result switch
        {
            DbKeys.Footprints => s => GetValues<Footprint>(s),
            DbKeys.GroundTargets => s => GetValues<GroundTarget>(s),
            DbKeys.Satellites => s => GetValues<Satellite>(s),
            DbKeys.GroundStations => s => GetValues<GroundStation>(s),
            DbKeys.UserGeometries => s => GetValues<UserGeometry>(s),
            _ => throw new Exception($"DbHelper key={key} not register."),
        };
    }

    public static Func<string, IList<object>> JsonReaderFromPath(string key)
    {
        Enum.TryParse<DbKeys>(key, true, out var result);
        return result switch
        {
            DbKeys.Footprints => s => GetValues<Footprint>(s),
            DbKeys.GroundTargets => s => GetValues<GroundTarget>(s),
            DbKeys.Satellites => s => GetValues<Satellite>(s),
            DbKeys.GroundStations => s => GetValues<GroundStation>(s),
            DbKeys.UserGeometries => s => GetValues<UserGeometry>(s),
            _ => throw new Exception($"DbHelper key={key} not register."),
        };
    }

    public static Func<IList<string>, IList<object>> Loader(string key)
    {
        Enum.TryParse<DbKeys>(key, true, out var result);

        return result switch
        {
            DbKeys.Maps => MapResource.Builder,
            DbKeys.FootprintPreviews => FootprintPreview.Builder,
            DbKeys.FootprintPreviewGeometries => FootprintPreviewGeometry.Builder,
            _ => throw new Exception($"DbHelper key={key} not register."),
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

    public static async Task<bool> JsonValidationAsync(string key, string path)
    {
        return await Task.Run(() =>
        {
            Enum.TryParse<DbKeys>(key, true, out var result);

            return result switch
            {
                DbKeys.Footprints => JsonValidation<List<Footprint>>(path),
                DbKeys.GroundTargets => JsonValidation<List<GroundTarget>>(path),
                DbKeys.Satellites => JsonValidation<List<Satellite>>(path),
                DbKeys.GroundStations => JsonValidation<List<GroundStation>>(path),
                DbKeys.UserGeometries => JsonValidation<List<UserGeometry>>(path),
                _ => throw new Exception($"DbHelper key={key} not register.")
            };
        });
    }

    private static IList<object> GetValues<T>(IList<string> paths)
    {
        return paths.SelectMany(s => GetValues<T>(s)).ToList();
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
