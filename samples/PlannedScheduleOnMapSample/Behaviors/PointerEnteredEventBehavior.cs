using Avalonia;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
using System.Windows.Input;

namespace PlannedScheduleOnMapSample.Behaviors;

public class PointerEnteredEventBehavior : Behavior<Interactive>
{
    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<PointerEnteredEventBehavior, ICommand?>(nameof(Command), null, false, BindingMode.OneWay, coerce: (s, value) => value is { } ? value : null);

    public ICommand? Command
    {
        get { return GetValue(CommandProperty); }
        set { SetValue(CommandProperty, value); }
    }

    public static readonly StyledProperty<RoutingStrategies> RoutingStrategiesProperty =
        AvaloniaProperty.Register<PointerEnteredEventBehavior, RoutingStrategies>(
            nameof(RoutingStrategies),
            RoutingStrategies.Direct);

    public RoutingStrategies RoutingStrategies
    {
        get => GetValue(RoutingStrategiesProperty);
        set => SetValue(RoutingStrategiesProperty, value);
    }

    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(InputElement.PointerEnteredEvent, PointerEnter, RoutingStrategies);
    }

    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(InputElement.PointerEnteredEvent, PointerEnter);
    }

    private void PointerEnter(object? sender, PointerEventArgs e)
    {
        OnPointerEnter(sender, e);
    }

    protected virtual void OnPointerEnter(object? sender, PointerEventArgs e)
    {
        if (sender is StyledElement element)
        {
            Command?.Execute(element.DataContext);
        }
    }
}
