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
        var source = new FootprintRandomSource()
        {
            GenerateCount = generateCount
        };

        var res = await source.GetValuesAsync();

        var footprints = res.Cast<Footprint>().ToList();

        return GroundTargetBuilder.Create(footprints).ToList();
    }
}
