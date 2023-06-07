using Mapsui;
using Mapsui.Layers;
using Mapsui.Styles;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SpaceScienceTest.Layers;

public class QueueLayer : BaseLayer, IModifyFeatureLayer
{
    private readonly ConcurrentQueue<IFeature> _cache = new();

    public override IEnumerable<IFeature> GetFeatures(MRect? box, double resolution)
    {
        // Safeguard in case MRect is null, most likely due to no features in layer
        if (box == null) return new List<IFeature>();
        var cache = _cache;
        var biggerBox = box.Grow(SymbolStyle.DefaultWidth * 2 * resolution, SymbolStyle.DefaultHeight * 2 * resolution);
        var result = cache.Where(f => biggerBox.Intersects(f.Extent));
        return result;
    }

    private MRect? GetExtent()
    {
        // todo: Calculate extent only once. Use a _modified field to determine when this is needed.

        var extents = _cache
            .Select(f => f.Extent)
            .Where(g => g != null)
            .ToList();

        if (extents.Count == 0) return null;

        var minX = extents.Min(g => g!.MinX);
        var minY = extents.Min(g => g!.MinY);
        var maxX = extents.Max(g => g!.MaxX);
        var maxY = extents.Max(g => g!.MaxY);

        return new MRect(minX, minY, maxX, maxY);
    }

    public override MRect? Extent => GetExtent();

    public IEnumerable<IFeature> GetFeatures()
    {
        return _cache;
    }

    public void Clear()
    {
        _cache.Clear();
    }

    public void Add(IFeature feature)
    {
        _cache.Enqueue(feature);
    }

    public void AddRange(IEnumerable<IFeature> features)
    {
        foreach (var feature in features)
        {
            _cache.Enqueue(feature);
        }
    }

    public IFeature? Find(IFeature feature)
    {
        return _cache.FirstOrDefault(f => f == feature);
    }
}