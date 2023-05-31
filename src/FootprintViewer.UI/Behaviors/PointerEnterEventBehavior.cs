using Avalonia;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
using System.Windows.Input;

namespace FootprintViewer.UI.Behaviors;

public class PointerEnterEventBehavior : Behavior<Interactive>
{
    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<PointerEnterEventBehavior, ICommand?>(nameof(Command), null, false, BindingMode.OneWay, coerce: (s, value) => value is { } ? value : null);

    public ICommand? Command
    {
        get { return GetValue(CommandProperty); }
        set { SetValue(CommandProperty, value); }
    }

    public static readonly StyledProperty<RoutingStrategies> RoutingStrategiesProperty =
        AvaloniaProperty.Register<PointerEnterEventBehavior, RoutingStrategies>(
            nameof(RoutingStrategies),
            RoutingStrategies.Direct);

    public RoutingStrategies RoutingStrategies
    {
        get => GetValue(RoutingStrategiesProperty);
        set => SetValue(RoutingStrategiesProperty, value);
    }

    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(InputElement.PointerEnterEvent, PointerEnter, RoutingStrategies);
    }

    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(InputElement.PointerEnterEvent, PointerEnter);
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
