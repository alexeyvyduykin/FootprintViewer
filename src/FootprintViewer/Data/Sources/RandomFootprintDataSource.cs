using FootprintViewer.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class RandomFootprintDataSource : IFootprintDataSource
    {
        private List<Footprint>? _footprints;
        private readonly ISatelliteDataSource _source;

        public RandomFootprintDataSource(ISatelliteDataSource source)
        {
            _source = source;
        }

        public async Task<List<Footprint>> GetFootprintsAsync()
        {
            return await Task.Run(async () =>
            {
                if (_footprints == null)
                {
                    var satellites = await _source.GetSatelliteInfosAsync();

                    _footprints = new List<Footprint>(FootprintBuilder.Create(satellites.Select(s => s.Satellite)));
                }

                return _footprints;
            });
        }

        public async Task<List<FootprintInfo>> GetFootprintInfosAsync()
        {
            return await Task.Run(async () =>
            {
                if (_footprints == null)
                {
                    var satellites = await _source.GetSatelliteInfosAsync();

                    _footprints = new List<Footprint>(FootprintBuilder.Create(satellites.Select(s => s.Satellite)));
                }

                return _footprints.Select(s => new FootprintInfo(s)).ToList();
            });
        }
    }
}
