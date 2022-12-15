using Mapsui.Styles;
using System;

namespace FootprintViewer.Styles;

public class LayerStyleViewModel
{
    private readonly Func<IStyle> _create;
    private IStyle? _style;

    public LayerStyleViewModel(string name, Func<IStyle> create)
    {
        Name = name;
        _create = create;
    }

    public LayerStyleViewModel(string name, string group, Func<IStyle> create)
    {
        Name = name;
        Group = group;
        _create = create;
    }

    public string Name { get; }

    public string? Group { get; }

    public IStyle? GetStyle() => _style ??= _create.Invoke();
}
