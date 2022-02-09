using FootprintViewer.Data.Sources;
using ReactiveUI;
using System.Collections.Generic;
using System.Reactive;

namespace FootprintViewer.Data
{
    public class GroundTargetProvider : BaseProvider<IGroundTargetDataSource>
    {
        public GroundTargetProvider()
        {
            Loading = ReactiveCommand.Create(() => GetGroundTargets());
        }

        public ReactiveCommand<Unit, IEnumerable<GroundTarget>> Loading { get; }

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
