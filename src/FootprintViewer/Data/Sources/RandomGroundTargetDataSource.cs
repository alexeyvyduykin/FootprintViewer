using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Data.Sources
{
    public class RandomGroundTargetDataSource : IGroundTargetDataSource
    {
        private IEnumerable<GroundTarget>? _groundTargets;
        private readonly IFootprintDataSource _source;

        public RandomGroundTargetDataSource(IFootprintDataSource source)
        {
            _source = source;      
        }

        public IEnumerable<GroundTarget> GetGroundTargets() => 
            _groundTargets ??= GroundTargetBuilder.Create(_source.GetFootprints().ToList());
    }
}
