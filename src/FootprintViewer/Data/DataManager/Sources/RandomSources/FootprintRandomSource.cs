using FootprintViewer.Data.Managers;
using FootprintViewer.Data.Sources;
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
        var manager = (IDataManager<Satellite>)new RandomSatelliteDataManager();

        var satellites = await manager.GetNativeValuesAsync(new RandomSource() { GenerateCount = 5 }, null);

        return FootprintBuilder.Create(satellites).Take(generateCount).ToList();
    }
}
