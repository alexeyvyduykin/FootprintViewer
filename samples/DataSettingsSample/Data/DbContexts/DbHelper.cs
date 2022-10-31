using Avalonia;
using Avalonia.Platform;
using DataSettingsSample.Models;
using FDataSettingsSample.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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
                DbKeys.Footprints => s => new FootprintDbContext(s, tableName),
                DbKeys.GroundTargets => s => new GroundTargetDbContext(s, tableName),
                DbKeys.Satellites => s => new SatelliteDbContext(s, tableName),
                DbKeys.GroundStations => s => new GroundStationDbContext(s, tableName),
                DbKeys.UserGeometries => s => new UserGeometryDbContext(s, tableName),
                _ => throw new Exception(),
            };
        }

        public static bool TryValidateContext(DbKeys key, string connectionString, string tableName)
        {
            using var context = CreateDatabaseSource(key, tableName).Invoke(connectionString);
            return context.Valid(GetType(key));
        }

        public static Type GetType(DbKeys key)
        {
            return key switch
            {
                DbKeys.Footprints => typeof(Footprint),
                DbKeys.GroundTargets => typeof(GroundTarget),
                DbKeys.Satellites => typeof(Satellite),
                DbKeys.GroundStations => typeof(GroundStation),
                DbKeys.UserGeometries => typeof(UserGeometry),
                _ => throw new Exception(),
            };
        }

        public static Func<IList<string>, IList<object>> JsonReaderFromPaths(DbKeys key)
        {
            return key switch
            {
                DbKeys.Footprints => s => GetValues<Footprint>(s),
                DbKeys.GroundTargets => s => GetValues<GroundTarget>(s),
                DbKeys.Satellites => s => GetValues<Satellite>(s),
                DbKeys.GroundStations => s => GetValues<GroundStation>(s),
                DbKeys.UserGeometries => s => GetValues<UserGeometry>(s),
                _ => throw new Exception(),
            };
        }

        public static Func<string, IList<object>> JsonReaderFromPath(DbKeys key)
        {
            return key switch
            {
                DbKeys.Footprints => s => GetValues<Footprint>(s),
                DbKeys.GroundTargets => s => GetValues<GroundTarget>(s),
                DbKeys.Satellites => s => GetValues<Satellite>(s),
                DbKeys.GroundStations => s => GetValues<GroundStation>(s),
                DbKeys.UserGeometries => s => GetValues<UserGeometry>(s),
                _ => throw new Exception(),
            };
        }

        public static Func<Uri, IList<object>> JsonReaderFromUri(DbKeys key)
        {
            return key switch
            {
                DbKeys.Footprints => s => GetValues<Footprint>(s),
                DbKeys.GroundTargets => s => GetValues<GroundTarget>(s),
                DbKeys.Satellites => s => GetValues<Satellite>(s),
                DbKeys.GroundStations => s => GetValues<GroundStation>(s),
                DbKeys.UserGeometries => s => GetValues<UserGeometry>(s),
                _ => throw new Exception(),
            };
        }

        public static Regex JsonNamePattern(DbKeys key)
        {
            return key switch
            {
                DbKeys.Footprints => new Regex("^(Fp_)"),
                DbKeys.GroundTargets => new Regex("^(Gt_)"),
                DbKeys.Satellites => new Regex("^(St_)"),
                DbKeys.GroundStations => new Regex("^(Gs_)"),
                DbKeys.UserGeometries => new Regex("^(Ug_)"),
                _ => throw new Exception(),
            };
        }

        public static bool JsonValidation(DbKeys key, string jsonString)
        {
            switch (key)
            {
                case DbKeys.Footprints:
                    {
                        var list = JsonConvert.DeserializeObject<List<Footprint>>(jsonString);
                        var name = list?.FirstOrDefault()?.Name;
                        var res = JsonNamePattern(key).IsMatch(name ?? string.Empty);
                        return res;
                    }
                case DbKeys.GroundTargets:
                    {
                        var list = JsonConvert.DeserializeObject<List<GroundTarget>>(jsonString);
                        var name = list?.FirstOrDefault()?.Name;
                        var res = JsonNamePattern(key).IsMatch(name ?? string.Empty);
                        return res;
                    }
                case DbKeys.Satellites:
                    {
                        var list = JsonConvert.DeserializeObject<List<Satellite>>(jsonString);
                        var name = list?.FirstOrDefault()?.Name;
                        var res = JsonNamePattern(key).IsMatch(name ?? string.Empty);
                        return res;
                    }
                case DbKeys.GroundStations:
                    {
                        var list = JsonConvert.DeserializeObject<List<GroundStation>>(jsonString);
                        var name = list?.FirstOrDefault()?.Name;
                        var res = JsonNamePattern(key).IsMatch(name ?? string.Empty);
                        return res;
                    }
                case DbKeys.UserGeometries:
                    {
                        var list = JsonConvert.DeserializeObject<List<UserGeometry>>(jsonString);
                        var name = list?.FirstOrDefault()?.Name;
                        var res = JsonNamePattern(key).IsMatch(name ?? string.Empty);
                        return res;
                    }
                default:
                    throw new Exception();
            };
        }

        private static IList<object> GetValues<T>(IList<string> paths)
        {
            return paths.SelectMany(s => DeserializeFromStream<T>(s).Cast<object>()).ToList();
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
