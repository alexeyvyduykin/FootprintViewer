using Mapsui.Styles;
using System;

namespace FootprintViewer.Styles;

public class LayerStyleViewModel
{
    private readonly Func<IPalette?, IStyle> _create;
    private IStyle? _style;
    private readonly IPalette? _palette;

    public LayerStyleViewModel(string name, IPalette? palette, Func<IPalette?, IStyle> create)
    {
        Name = name;
        _palette = palette;
        _create = create;
    }

    public LayerStyleViewModel(string name, string group, IPalette? palette, Func<IPalette?, IStyle> create)
    {
        Name = name;
        Group = group;
        _palette = palette;
        _create = create;
    }

    public string Name { get; }

    public string? Group { get; }

    public IPalette? Palette => _palette;

    public IStyle? GetStyle() => _style ??= _create.Invoke(_palette);
}
