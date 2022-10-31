using DataSettingsSample.Data;
using DataSettingsSample.Models;
using FDataSettingsSample.Models;
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
        private static readonly int[] values3 = new[] { 323322, 344645, 127343, 478333, 324332 };
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
                var source1 = new LocalSource<Footprint>(values1.Select(s => new Footprint() { Name = $"Fp_{new Random().Next(1, 100)}", Value = s }).ToList());
                var source2 = new LocalSource<GroundTarget>(values2.Select(s => new GroundTarget() { Name = $"Gt_{new Random().Next(1, 100)}", Value = s }).ToList());
                var source3 = new LocalSource<Satellite>(values3.Select(s => new Satellite() { Name = $"St_{new Random().Next(1, 100)}", ValueInt = s }).ToList());
                var source4 = new LocalSource<GroundStation>(values4.Select(s => new GroundStation() { Name = $"Gs_{new Random().Next(1, 100)}", Value = s }).ToList());
                var source5 = new LocalSource<UserGeometry>(values5.Select(s => new UserGeometry() { Name = $"Ug_{new Random().Next(1, 100)}", Value = s }).ToList());

                RegisterSource(DbKeys.Footprints.ToString(), source1);
                RegisterSource(DbKeys.GroundTargets.ToString(), source2);
                RegisterSource(DbKeys.Satellites.ToString(), source3);
                RegisterSource(DbKeys.GroundStations.ToString(), source4);
                RegisterSource(DbKeys.UserGeometries.ToString(), source5);
            }
        }

        public IEnumerable<object> GetServices(Type? serviceType, string? contract = null)
        {
            throw new Exception();
        }
    }

    internal class LocalSource<T> : ISource
    {
        private readonly List<T> _list;

        public LocalSource(List<T> list)
        {
            _list = list;
        }

        public IList<object> GetValues() => _list.Cast<object>().ToList();

        public async Task<IList<object>> GetValuesAsync() => await Task.Run(() => _list.Cast<object>().ToList());
    }
}
