using FootprintViewer.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class RandomFootprintDataSource : IDataSource<FootprintInfo>
    {
        private List<FootprintInfo>? _footprints;
        private readonly IDataSource<SatelliteInfo> _source;

        public RandomFootprintDataSource(IDataSource<SatelliteInfo> source)
        {
            _source = source;
        }

        public async Task<List<FootprintInfo>> GetValuesAsync(IFilter<FootprintInfo>? filter = null)
        {
            return await Task.Run(async () =>
            {
                if (_footprints == null)
                {
                    var satellites = await _source.GetValuesAsync(null);

                    var footprints = FootprintBuilder.Create(satellites.Select(s => s.Satellite));

                    _footprints = footprints.Select(s => new FootprintInfo(s)).ToList();
                }

                return _footprints;
            });
        }
    }
}
