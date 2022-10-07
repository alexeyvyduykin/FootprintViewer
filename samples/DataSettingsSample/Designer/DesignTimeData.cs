using DataSettingsSample.Data;
using DataSettingsSample.ViewModels;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataSettingsSample.Designer
{
    public class DesignTimeData : IReadonlyDependencyResolver
    {
        private static readonly double[] values1 = new[] { 542346565.3454, 56534343.6442, 9304038592.4331, 89023437112.9033, 7882343023.6033 };
        private static readonly double[] values2 = new[] { 945877230.0542, 99723677.1293, 5093576821.9304, 39984376722.0343, 1928398435.3022 };
        private static readonly double[] values3 = new[] { 323943222.2932, 34493945.2233, 1237773343.8773, 32343478333.9823, 3223384332.0932 };
        private static readonly double[] values4 = new[] { 214343894.3439, 32299843.9843, 2938473833.0234, 34990234322.1092, 2987433544.0923 };
        private static readonly double[] values5 = new[] { 398653243.2233, 12096320.3764, 3475235478.3676, 23887122344.5744, 2345984487.2974 };
        private Repository? _repository;

        public object? GetService(Type? serviceType, string? contract = null)
        {
            if (serviceType == typeof(Repository))
            {
                return _repository ??= new DesignTimeRepository();
            }

            throw new Exception();
        }

        private class DesignTimeRepository : Repository
        {
            public DesignTimeRepository()
            {
                var source1 = new LocalSource(new CustomJsonObject() { Key = "Footprints", Values = new List<double>(values1) });
                var source2 = new LocalSource(new CustomJsonObject() { Key = "GroundTargets", Values = new List<double>(values2) });
                var source3 = new LocalSource(new CustomJsonObject() { Key = "Satellites", Values = new List<double>(values3) });
                var source4 = new LocalSource(new CustomJsonObject() { Key = "GroundStations", Values = new List<double>(values4) });
                var source5 = new LocalSource(new CustomJsonObject() { Key = "UserGeometries", Values = new List<double>(values5) });

                RegisterSource("footprints", source1);
                RegisterSource("groundTargets", source2);
                RegisterSource("satellites", source3);
                RegisterSource("groundStations", source4);
                RegisterSource("userGeometries", source5);
            }
        }

        private class LocalSource : ISource
        {
            private readonly CustomJsonObject _cache;

            public LocalSource(CustomJsonObject obj)
            {
                _cache = obj;
            }

            public IList<object> GetValues() => new List<CustomJsonObject>() { _cache }.Cast<object>().ToList();

            public async Task<IList<object>> GetValuesAsync() => await Task.Run(() => new List<CustomJsonObject>() { _cache }.Cast<object>().ToList());
        }

        public IEnumerable<object> GetServices(Type? serviceType, string? contract = null)
        {
            throw new Exception();
        }
    }
}
