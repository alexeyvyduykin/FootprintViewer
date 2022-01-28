using FootprintViewer.Data.Sources;
using System.Collections.Generic;

namespace FootprintViewer.Data
{
    public class GroundTargetProvider
    {
        private readonly List<IGroundTargetDataSource> _sources;

        public GroundTargetProvider()
        {
            _sources = new List<IGroundTargetDataSource>();
        }

        public void AddSource(IGroundTargetDataSource source)
        {
            _sources.Add(source);
        }

        public IEnumerable<GroundTarget> GetGroundTargets()
        {
            var list = new List<GroundTarget>();

            foreach (var source in Sources)
            {
                list.AddRange(source.GetGroundTargets());
            }

            return list;
        }

        public IEnumerable<IGroundTargetDataSource> Sources => _sources;
    }
}
