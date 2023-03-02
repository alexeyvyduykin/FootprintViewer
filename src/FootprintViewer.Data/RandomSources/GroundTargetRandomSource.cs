using FootprintViewer.Data.Builders;
using FootprintViewer.Data.Models;

namespace FootprintViewer.Data.RandomSources;

public class GroundTargetRandomSource : BaseRandomSource
{
    private IList<GroundTarget>? _groundTargets;

    public IList<Footprint>? Footprints { get; set; }

    public override async Task<IList<object>> GetValuesAsync()
    {
        return await Task.Run(async () =>
        {
            if (Footprints == null)
            {
                var source = new FootprintRandomSource()
                {
                    GenerateCount = 2000
                };

                var res = await source.GetValuesAsync();

                Footprints = res.Cast<Footprint>().ToList();
            }

            _groundTargets ??= await Build(Footprints, GenerateCount);

            return _groundTargets.Cast<object>().ToList();
        });
    }

    private static async Task<IList<GroundTarget>> Build(IList<Footprint> footprints, int generateCount)
    {
        return await Task.Run(() => GroundTargetBuilder.Create(footprints, generateCount).ToList());
    }
}
