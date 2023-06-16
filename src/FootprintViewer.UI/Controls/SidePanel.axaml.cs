using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Media;
using System.Collections;

namespace FootprintViewer.UI.Controls;

public class SidePanel : TabControl
{
    public SidePanel() : base()
    {
        AffectsMeasure<SidePanel>(IsExpandedProperty);
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new SidePanelItem();
    }

    public static readonly StyledProperty<bool> IsExpandedProperty =
        AvaloniaProperty.Register<SidePanel, bool>(nameof(IsExpanded), true);

    public bool IsExpanded
    {
        get => GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    public static readonly StyledProperty<IBrush?> PaneBackgroundProperty =
        SplitView.PaneBackgroundProperty.AddOwner<SidePanel>();

    public IBrush? PaneBackground
    {
        get => GetValue(PaneBackgroundProperty);
        set => SetValue(PaneBackgroundProperty, value);
    }

    public static readonly StyledProperty<IBrush> ContentBackgroundProperty =
        AvaloniaProperty.Register<SidePanel, IBrush>(nameof(ContentBackground));

    public IBrush ContentBackground
    {
        get => GetValue(ContentBackgroundProperty);
        set => SetValue(ContentBackgroundProperty, value);
    }

    public static readonly StyledProperty<IBrush> TitleBackgroundProperty =
        AvaloniaProperty.Register<SidePanel, IBrush>(nameof(TitleBackground));

    public IBrush TitleBackground
    {
        get => GetValue(TitleBackgroundProperty);
        set => SetValue(TitleBackgroundProperty, value);
    }

    public static readonly StyledProperty<double> PaneTitleHeightProperty =
        AvaloniaProperty.Register<SidePanel, double>(nameof(PaneTitleHeight), 50);

    public double PaneTitleHeight
    {
        get => GetValue(PaneTitleHeightProperty);
        set => SetValue(PaneTitleHeightProperty, value);
    }

    public static readonly StyledProperty<double> PaneWidthProperty =
        AvaloniaProperty.Register<SidePanel, double>(nameof(PaneWidth), 100);

    public double PaneWidth
    {
        get => GetValue(PaneWidthProperty);
        set => SetValue(PaneWidthProperty, value);
    }

    private IEnumerable _actionTabs = new AvaloniaList<object>();

    public static readonly DirectProperty<SidePanel, IEnumerable> ActionTabsProperty =
        AvaloniaProperty.RegisterDirect<SidePanel, IEnumerable>(nameof(ActionTabs), o => o.ActionTabs, (o, v) => o.ActionTabs = v);

    public IEnumerable ActionTabs
    {
        get { return _actionTabs; }
        set { SetAndRaise(ActionTabsProperty, ref _actionTabs, value); }
    }
}
