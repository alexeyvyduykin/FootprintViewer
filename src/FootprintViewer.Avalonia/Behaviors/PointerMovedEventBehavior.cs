using Avalonia;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using System.Windows.Input;

namespace FootprintViewer.Avalonia.Behaviors;

public class PointerMovedEventBehavior : Behavior<Interactive>
{
    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<PointerMovedEventBehavior, ICommand?>(nameof(Command), null, false, BindingMode.OneWay, coerce: (s, value) => value is { } ? value : null);

    public ICommand? Command
    {
        get { return GetValue(CommandProperty); }
        set { SetValue(CommandProperty, value); }
    }

    public static readonly StyledProperty<RoutingStrategies> RoutingStrategiesProperty =
        AvaloniaProperty.Register<PointerMovedEventBehavior, RoutingStrategies>(
            nameof(RoutingStrategies),
            RoutingStrategies.Bubble);

    public RoutingStrategies RoutingStrategies
    {
        get => GetValue(RoutingStrategiesProperty);
        set => SetValue(RoutingStrategiesProperty, value);
    }

    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(InputElement.PointerMovedEvent, PointerMoved, RoutingStrategies);
    }

    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(InputElement.PointerMovedEvent, PointerMoved);
    }

    private void PointerMoved(object? sender, PointerEventArgs e)
    {
        OnPointerMoved(sender, e);
    }

    protected virtual void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (sender is IVisual visual)
        {
            var screenPosition = e.GetPosition(visual);

            Command?.Execute((screenPosition.X, screenPosition.Y));
        }
    }
}
