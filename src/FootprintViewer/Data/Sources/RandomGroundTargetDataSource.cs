using System.Collections.Generic;

namespace FootprintViewer.Data.Sources
{
    public class RandomGroundTargetDataSource : IGroundTargetDataSource
    {
        private readonly IEnumerable<GroundTarget> _groundTargets;

        public RandomGroundTargetDataSource(IList<Footprint> footprints)
        {
            _groundTargets = GroundTargetBuilder.Create(footprints);
        }

        public IEnumerable<GroundTarget> GetGroundTargets() => _groundTargets;
    }
}
