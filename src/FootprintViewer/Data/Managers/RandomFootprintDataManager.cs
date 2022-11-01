using FootprintViewer.Data.DataManager.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Managers;

public class RandomFootprintDataManager : BaseDataManager<Footprint, IRandomSource>
{
    private List<Footprint>? _footprints;

    protected override async Task<List<Footprint>> GetNativeValuesAsync(IRandomSource dataSource, IFilter<Footprint>? filter)
    {
        return await Task.Run(async () =>
        {
            _footprints ??= await Build(dataSource);

            return _footprints;
        });
    }

    protected override async Task<List<T>> GetValuesAsync<T>(IRandomSource dataSource, IFilter<T>? filter, Func<Footprint, T> converter)
    {
        return await Task.Run(async () =>
        {
            _footprints ??= await Build(dataSource);

            return _footprints.Select(s => converter(s)).ToList();
        });
    }

    private static async Task<List<Footprint>> Build(IRandomSource dataSource)
    {
        var source = new SatelliteRandomSource()
        {
            GenerateCount = 5
        };

        var res = await source.GetValuesAsync();

        var satellites = res.Cast<Satellite>().ToList();

        return FootprintBuilder.Create(satellites).Take(dataSource.GenerateCount).ToList();
    }
}
