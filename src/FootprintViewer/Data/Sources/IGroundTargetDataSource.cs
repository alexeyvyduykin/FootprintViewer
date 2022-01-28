using System.Collections.Generic;

namespace FootprintViewer.Data.Sources
{
    public interface IGroundTargetDataSource
    {
        IEnumerable<GroundTarget> GetGroundTargets();
    }
}
