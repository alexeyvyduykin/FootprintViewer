using Avalonia;
using Avalonia.Platform;
using DataSettingsSample.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DataSettingsSample.Data
{
    public static class DbHelper
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

        public static Func<string, DbCustomContext> CreateDatabaseSource(DbKeys key, string tableName)
        {
            return key switch
            {
                DbKeys.Footprints => s => new FootprintDbContext(s, tableName/*"Footprints"*/),
                DbKeys.GroundTargets => s => new GroundTargetDbContext(s, tableName/*"GroundTargets"*/),
                DbKeys.Satellites => s => new SatelliteDbContext(s, tableName/*"Satellites"*/),
                DbKeys.GroundStations => s => new GroundStationDbContext(s, tableName/*"GroundStations"*/),
                DbKeys.UserGeometries => s => new UserGeometryDbContext(s, tableName/*"UserGeometries"*/),
                _ => throw new Exception(),
            };
        }

        public static Func<IList<string>, IList<object>> JsonReaderFromPaths(DbKeys key)
        {
            return key switch
            {
                DbKeys.Footprints => s => GetValues<CustomJsonObject>(s),
                DbKeys.GroundTargets => s => GetValues<CustomJsonObject>(s),
                DbKeys.Satellites => s => GetValues<CustomJsonObject>(s),
                DbKeys.GroundStations => s => GetValues<CustomJsonObject>(s),
                DbKeys.UserGeometries => s => GetValues<CustomJsonObject>(s),
                _ => throw new Exception(),
            };
        }

        public static Func<string, IList<object>> JsonReaderFromPath(DbKeys key)
        {
            return key switch
            {
                DbKeys.Footprints => s => GetValues<CustomJsonObject>(s),
                DbKeys.GroundTargets => s => GetValues<CustomJsonObject>(s),
                DbKeys.Satellites => s => GetValues<CustomJsonObject>(s),
                DbKeys.GroundStations => s => GetValues<CustomJsonObject>(s),
                DbKeys.UserGeometries => s => GetValues<CustomJsonObject>(s),
                _ => throw new Exception(),
            };
        }

        public static Func<Uri, IList<object>> JsonReaderFromUri(DbKeys key)
        {
            return key switch
            {
                DbKeys.Footprints => s => GetValues<CustomJsonObject>(s),
                DbKeys.GroundTargets => s => GetValues<CustomJsonObject>(s),
                DbKeys.Satellites => s => GetValues<CustomJsonObject>(s),
                DbKeys.GroundStations => s => GetValues<CustomJsonObject>(s),
                DbKeys.UserGeometries => s => GetValues<CustomJsonObject>(s),
                _ => throw new Exception(),
            };
        }

        private static IList<object> GetValues<T>(IList<string> paths)
        {
            return paths.SelectMany(s => DeserializeFromStream<CustomJsonObject>(s).Cast<object>()).ToList();
        }

        private static IList<object> GetValues<T>(string path)
        {
            return DeserializeFromStream<T>(path).Cast<object>().ToList();
        }

        private static IList<object> GetValues<T>(Uri uri)
        {
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            var stream = assets?.Open(uri)!;
            return DeserializeFromStream<T>(stream).Cast<object>().ToList();
        }
    }
}
