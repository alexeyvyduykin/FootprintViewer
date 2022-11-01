using FootprintViewer.Data.Managers;
using FootprintViewer.Data.Sources;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.DataManager.Sources;

public class GroundTargetRandomSource : BaseRandomSource
{
    private IList<GroundTarget>? _groundTargets;

    public override async Task<IList<object>> GetValuesAsync()
    {
        return await Task.Run(async () =>
        {
            _groundTargets ??= await Build(GenerateCount);

            return _groundTargets.Cast<object>().ToList();
        });
    }

    private static async Task<IList<GroundTarget>> Build(int generateCount)
    {
        var manager = (IDataManager<Footprint>)new RandomFootprintDataManager();

        var footprints = await manager.GetNativeValuesAsync(new RandomSource() { GenerateCount = generateCount }, null);

        return GroundTargetBuilder.Create(footprints).ToList();
    }
}
