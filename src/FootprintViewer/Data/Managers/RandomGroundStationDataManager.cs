using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Managers
{
    public class RandomGroundStationDataManager : BaseDataManager<GroundStation, IRandomSource>
    {
        private List<GroundStation>? _groundStations;
        private readonly List<GroundStation> _allGroundStations;

        public RandomGroundStationDataManager()
        {
            _allGroundStations = new List<GroundStation>()
            {
                new GroundStation() { Name = "Москва",      Center = new Point( 37.38, 55.56), Angles = new [] { 0.0, 6, 10, 11 } },
                new GroundStation() { Name = "Новосибирск", Center = new Point( 82.57, 54.59), Angles = new [] { 0.0, 6, 10, 11 } },
                new GroundStation() { Name = "Хабаровск",   Center = new Point(135.04, 48.29), Angles = new [] { 0.0, 6, 10, 11 } },
                new GroundStation() { Name = "Шпицберген",  Center = new Point(    21, 78.38), Angles = new [] { 0.0, 6, 10, 11 } },
                new GroundStation() { Name = "Анадырь",     Center = new Point(177.31, 64.44), Angles = new [] { 0.0, 6, 10, 11 } },
                new GroundStation() { Name = "Тикси",       Center = new Point(128.52, 71.38), Angles = new [] { 0.0, 6, 10, 11 } },
            };
        }

        protected override async Task<List<GroundStation>> GetNativeValuesAsync(IRandomSource dataSource, IFilter<GroundStation>? filter)
        {
            return await Task.Run(() => _groundStations ??= BuildGroundStations(dataSource));
        }

        protected override async Task<List<T>> GetValuesAsync<T>(IRandomSource dataSource, IFilter<T>? filter, Func<GroundStation, T> converter)
        {
            _groundStations ??= BuildGroundStations(dataSource);

            return await Task.Run(() => _groundStations.Select(s => converter(s)).ToList());
        }

        private List<GroundStation> BuildGroundStations(IRandomSource source)
        {
            return _allGroundStations.Take(source.GenerateCount).ToList();
        }
    }
}
