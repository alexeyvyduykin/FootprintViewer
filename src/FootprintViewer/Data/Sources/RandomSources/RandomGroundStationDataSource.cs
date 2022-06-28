using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class RandomGroundStationDataSource : IDataSource<GroundStation>
    {
        private readonly List<GroundStation> _groundStations;

        public RandomGroundStationDataSource()
        {
            _groundStations = new List<GroundStation>()
            {
                new GroundStation() { Name = "Москва",      Center = new Point( 37.38, 55.56), Angles = new [] { 0.0, 6, 10, 11 } },
                new GroundStation() { Name = "Новосибирск", Center = new Point( 82.57, 54.59), Angles = new [] { 0.0, 6, 10, 11 } },
                new GroundStation() { Name = "Хабаровск",   Center = new Point(135.04, 48.29), Angles = new [] { 0.0, 6, 10, 11 } },
                new GroundStation() { Name = "Шпицберген",  Center = new Point(    21, 78.38), Angles = new [] { 0.0, 6, 10, 11 } },
                new GroundStation() { Name = "Анадырь",     Center = new Point(177.31, 64.44), Angles = new [] { 0.0, 6, 10, 11 } },
                new GroundStation() { Name = "Тикси",       Center = new Point(128.52, 71.38), Angles = new [] { 0.0, 6, 10, 11 } },
            };
        }

        public async Task<List<GroundStation>> GetNativeValuesAsync(IFilter<GroundStation>? filter)
        {
            return await Task.Run(() => _groundStations);
        }

        public async Task<List<T>> GetValuesAsync<T>(IFilter<T>? filter, Func<GroundStation, T> converter)
        {
            return await Task.Run(() => _groundStations.Select(s => converter(s)).ToList());
        }
    }
}
