using Avalonia;
using Avalonia.Controls;

namespace FootprintViewer.Fluent.Controls;

public class Dialog : ContentControl
{
    public static readonly StyledProperty<bool> IsDialogOpenProperty =
        AvaloniaProperty.Register<Dialog, bool>(nameof(IsDialogOpen));

    public static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<Dialog, bool>(nameof(IsActive), coerce: (s, isActive) => s != null && isActive);

    public static readonly StyledProperty<bool> IsBackEnabledProperty =
        AvaloniaProperty.Register<Dialog, bool>(nameof(IsBackEnabled), coerce: (s, isActive) => s != null && isActive);

    public static readonly StyledProperty<bool> IsCancelEnabledProperty =
        AvaloniaProperty.Register<Dialog, bool>(nameof(IsCancelEnabled), coerce: (s, isActive) => s != null && isActive);

    public bool IsDialogOpen
    {
        get => GetValue(IsDialogOpenProperty);
        set => SetValue(IsDialogOpenProperty, value);
    }

    public bool IsActive
    {
        get => GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public bool IsBackEnabled
    {
        get => GetValue(IsBackEnabledProperty);
        set => SetValue(IsBackEnabledProperty, value);
    }

    public bool IsCancelEnabled
    {
        get => GetValue(IsCancelEnabledProperty);
        set => SetValue(IsCancelEnabledProperty, value);
    }

    protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsDialogOpenProperty)
        {
            PseudoClasses.Set(":open", change.NewValue.GetValueOrDefault<bool>());
        }
    }
}
