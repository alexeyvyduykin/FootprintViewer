using System.Collections.Generic;
using System.Threading.Tasks;
using nts = NetTopologySuite.Geometries;

namespace FootprintViewer.Data.Sources
{
    public class RandomGroundStationDataSource : IGroundStationDataSource
    {
        private readonly List<GroundStation> _groundStations;

        public RandomGroundStationDataSource()
        {
            _groundStations = new List<GroundStation>()
            {
                new GroundStation() { Name = "Москва",      Center = new nts.Point( 37.38, 55.56), Angles = new [] { 0.0, 6, 10, 11 } },
                new GroundStation() { Name = "Новосибирск", Center = new nts.Point( 82.57, 54.59), Angles = new [] { 0.0, 6, 10, 11 } },
                new GroundStation() { Name = "Хабаровск",   Center = new nts.Point(135.04, 48.29), Angles = new [] { 0.0, 6, 10, 11 } },
                new GroundStation() { Name = "Шпицберген",  Center = new nts.Point(    21, 78.38), Angles = new [] { 0.0, 6, 10, 11 } },
                new GroundStation() { Name = "Анадырь",     Center = new nts.Point(177.31, 64.44), Angles = new [] { 0.0, 6, 10, 11 } },
                new GroundStation() { Name = "Тикси",       Center = new nts.Point(128.52, 71.38), Angles = new [] { 0.0, 6, 10, 11 } },
            };
        }

        public async Task<List<GroundStation>> GetGroundStationsAsync()
        {
            return await Task.Run(() => _groundStations);
        }
    }
}
