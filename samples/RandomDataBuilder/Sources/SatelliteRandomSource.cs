using FootprintViewer.Data;

namespace RandomDataBuilder.Sources;

public class SatelliteRandomSource : BaseRandomSource
{
    private IList<Satellite>? _satellites;

    public override async Task<IList<object>> GetValuesAsync()
    {
        return await Task.Run(() =>
        {
            _satellites ??= BuildSatellites(GenerateCount);

            return _satellites.Cast<object>().ToList();
        });
    }

    private static IList<Satellite> BuildSatellites(int generateCount)
    {
        return new List<Satellite>(SatelliteBuilder.Create(generateCount));
    }
}
