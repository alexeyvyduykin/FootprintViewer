using Avalonia;
using Avalonia.Controls;

namespace FootprintViewer.Fluent.Controls;

public class SidePanelItem : TabItem
{

    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<SidePanelItem, string>(nameof(Title));

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
}
