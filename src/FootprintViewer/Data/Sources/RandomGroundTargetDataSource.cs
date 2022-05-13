using FootprintViewer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class RandomGroundTargetDataSource : IGroundTargetDataSource
    {
        private List<GroundTarget>? _groundTargets;
        private readonly IFootprintDataSource _source;

        public RandomGroundTargetDataSource(IFootprintDataSource source)
        {
            _source = source;
        }

        public async Task<List<GroundTarget>> GetGroundTargetsAsync()
        {
            return await Task.Run(async () =>
            {
                if (_groundTargets == null)
                {
                    var footprints = await _source.GetFootprintsAsync();

                    _groundTargets = new List<GroundTarget>(GroundTargetBuilder.Create(footprints));
                }

                return _groundTargets;
            });
        }

        public async Task<List<GroundTarget>> GetGroundTargetsAsync(string[] names)
        {
            return await Task.Run(async () =>
            {
                if (_groundTargets == null)
                {
                    var footprints = await _source.GetFootprintsAsync();

                    _groundTargets = new List<GroundTarget>(GroundTargetBuilder.Create(footprints));
                }

                return _groundTargets.Where(s => names.Contains(s.Name)).ToList();
            });
        }

        public async Task<List<GroundTargetInfo>> GetGroundTargetInfosAsync(IFilter<GroundTargetInfo>? filter)
        {
            return await Task.Run(async () =>
            {
                if (_groundTargets == null)
                {
                    var footprints = await _source.GetFootprintsAsync();

                    _groundTargets = new List<GroundTarget>(GroundTargetBuilder.Create(footprints));
                }

                if (filter == null)
                {
                    return _groundTargets.Select(s => new GroundTargetInfo(s)).ToList();
                }

                return _groundTargets.Select(s => new GroundTargetInfo(s)).Where(s => filter.Filtering(s)).ToList();
            });
        }
    }
}
