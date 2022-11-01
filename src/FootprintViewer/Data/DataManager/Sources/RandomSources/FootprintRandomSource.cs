using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.DataManager.Sources;

public class FootprintRandomSource : BaseRandomSource
{
    private IList<Footprint>? _footprints;

    public override async Task<IList<object>> GetValuesAsync()
    {
        return await Task.Run(async () =>
        {
            _footprints ??= await Build(GenerateCount);

            return _footprints.Cast<object>().ToList();
        });
    }

    private static async Task<IList<Footprint>> Build(int generateCount)
    {
        var source = new SatelliteRandomSource()
        {
            GenerateCount = 5
        };

        var res = await source.GetValuesAsync();

        var satellites = res.Cast<Satellite>().ToList();

        return FootprintBuilder.Create(satellites).Take(generateCount).ToList();
    }
}
