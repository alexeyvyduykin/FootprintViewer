using FootprintViewer.Data.Sources;
using ReactiveUI;
using System.Collections.Generic;
using System.Reactive;

namespace FootprintViewer.Data
{
    public class FootprintProvider : BaseProvider<IFootprintDataSource>
    {
        public FootprintProvider()
        {
            Loading = ReactiveCommand.Create(() => GetFootprints());
        }

        public ReactiveCommand<Unit, IEnumerable<Footprint>> Loading { get; }

        public IEnumerable<Footprint> GetFootprints()
        {
            var list = new List<Footprint>();

            foreach (var source in Sources)
            {
                list.AddRange(source.GetFootprints());
            }

            return list;
        }
    }
}
