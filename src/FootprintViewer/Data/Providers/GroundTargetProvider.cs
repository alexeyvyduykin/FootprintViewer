using FootprintViewer.Data.Sources;
using System.Collections.Generic;

namespace FootprintViewer.Data
{
    public class GroundTargetProvider : BaseProvider<IGroundTargetDataSource>
    {
        public IEnumerable<GroundTarget> GetGroundTargets()
        {
            var list = new List<GroundTarget>();

            foreach (var source in Sources)
            {
                list.AddRange(source.GetGroundTargets());
            }

            return list;
        }
    }
}
