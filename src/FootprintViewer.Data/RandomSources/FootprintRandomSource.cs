using FootprintViewer.Data.Builders;
using FootprintViewer.Data.Models;

namespace FootprintViewer.Data.RandomSources;

public class FootprintRandomSource : BaseRandomSource
{
    private IList<Footprint>? _footprints;

    public IList<Satellite>? Satellites { get; set; }

    public override async Task<IList<object>> GetValuesAsync()
    {
        return await Task.Run(async () =>
        {
            if (Satellites == null)
            {
                var source = new SatelliteRandomSource()
                {
                    GenerateCount = 5
                };

                var res = await source.GetValuesAsync();

                Satellites = res.Cast<Satellite>().ToList();
            }

            _footprints ??= await Build(Satellites, GenerateCount);

            return _footprints.Cast<object>().ToList();
        });
    }

    private static async Task<IList<Footprint>> Build(IList<Satellite> satellites, int generateCount)
    {
        return await Task.Run(() => FootprintBuilder.Create(satellites).Take(generateCount).ToList());
    }
}
