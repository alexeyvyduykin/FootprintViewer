using Mapsui.Styles;

namespace FootprintViewer.Styles;

public class MapsuiPalette : IMapsuiPalette
{
    private readonly Color _color;

    public MapsuiPalette(Color color)
    {
        _color = color;
    }

    public Color Color => _color;
}
