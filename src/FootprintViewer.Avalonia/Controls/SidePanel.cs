using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace FootprintViewer.Avalonia.Controls
{
    public class SidePanel : TabControl
    {

        public static readonly StyledProperty<bool> IsCompactProperty =
            AvaloniaProperty.Register<SidePanel, bool>(nameof(IsCompact), false);

        public bool IsCompact
        {
            get => GetValue(IsCompactProperty);
            set => SetValue(IsCompactProperty, value);
        }

        public static readonly StyledProperty<IBrush> PaneBackgroundProperty =
            SplitView.PaneBackgroundProperty.AddOwner<SidePanel>();

        public IBrush PaneBackground
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
    }
}
