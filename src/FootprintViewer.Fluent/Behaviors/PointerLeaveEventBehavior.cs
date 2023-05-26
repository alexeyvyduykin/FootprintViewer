using Avalonia;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
using System.Windows.Input;

namespace FootprintViewer.UI.Behaviors;

public class PointerLeaveEventBehavior : Behavior<Interactive>
{
    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<PointerLeaveEventBehavior, ICommand?>(nameof(Command), null, false, BindingMode.OneWay, coerce: (s, value) => value is { } ? value : null);

    public ICommand? Command
    {
        get { return GetValue(CommandProperty); }
        set { SetValue(CommandProperty, value); }
    }

    public static readonly StyledProperty<RoutingStrategies> RoutingStrategiesProperty =
        AvaloniaProperty.Register<PointerLeaveEventBehavior, RoutingStrategies>(
            nameof(RoutingStrategies),
            RoutingStrategies.Direct);

    public RoutingStrategies RoutingStrategies
    {
        get => GetValue(RoutingStrategiesProperty);
        set => SetValue(RoutingStrategiesProperty, value);
    }

    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(InputElement.PointerLeaveEvent, PointerLeave, RoutingStrategies);
    }

    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(InputElement.PointerLeaveEvent, PointerLeave);
    }

    private void PointerLeave(object? sender, PointerEventArgs e)
    {
        OnPointerLeave(sender, e);
    }

    protected virtual void OnPointerLeave(object? sender, PointerEventArgs e)
    {
        Command?.Execute(null);
    }
}
