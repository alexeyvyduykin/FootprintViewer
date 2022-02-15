using FootprintViewer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public async Task<List<GroundTargetInfo>> GetGroundTargetInfosAsync(string[] names)
        {
            return await Task.Run(async () =>
            {
                if (_groundTargets == null)
                {
                    var footprints = await _source.GetFootprintsAsync();

                    _groundTargets = new List<GroundTarget>(GroundTargetBuilder.Create(footprints));
                }

                return _groundTargets.Where(s => names.Contains(s.Name)).Select(s => new GroundTargetInfo(s)).ToList();
            });
        }

        public async Task<List<GroundTargetInfo>> GetGroundTargetInfosExAsync(Func<GroundTarget, bool> func)
        {
            return await Task.Run(async () =>
            {
                if (_groundTargets == null)
                {
                    var footprints = await _source.GetFootprintsAsync();

                    _groundTargets = new List<GroundTarget>(GroundTargetBuilder.Create(footprints));
                }

                return _groundTargets.Where(s => func(s)).Select(s => new GroundTargetInfo(s)).ToList();
            });
        }
    }
}
