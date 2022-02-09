using FootprintViewer.Data.Sources;
using ReactiveUI;
using System.Collections.Generic;
using System.Reactive;

namespace FootprintViewer.Data
{
    public class SatelliteProvider : BaseProvider<ISatelliteDataSource>
    {
        public SatelliteProvider()
        {
            Loading = ReactiveCommand.Create(GetSatellites);
        }

        public ReactiveCommand<Unit, IEnumerable<Satellite>> Loading { get; }

        public IEnumerable<Satellite> GetSatellites()
        {
            var list = new List<Satellite>();

            foreach (var source in Sources)
            {
                list.AddRange(source.GetSatellites());
            }

            return list;
        }
    }
}
