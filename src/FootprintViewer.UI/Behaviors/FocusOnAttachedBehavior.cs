using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using System.Reactive.Disposables;

namespace FootprintViewer.UI.Behaviors;

public class FocusOnAttachedBehavior : AttachedToVisualTreeBehavior<Control>
{
    public static readonly StyledProperty<bool> IsEnabledProperty =
        AvaloniaProperty.Register<FocusOnAttachedBehavior, bool>(nameof(IsEnabled), true);

    public bool IsEnabled
    {
        get => GetValue(IsEnabledProperty);
        set => SetValue(IsEnabledProperty, value);
    }

    protected override void OnAttachedToVisualTree(CompositeDisposable disposables)
    {
        if (IsEnabled)
        {
            Dispatcher.UIThread.Post(() => AssociatedObject?.Focus());
        }
    }
}
