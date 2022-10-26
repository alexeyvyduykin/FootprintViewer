using Avalonia;
using Avalonia.Controls;

namespace FootprintViewer.Avalonia.Controls;

public class ContentArea : ContentControl
{
    public static readonly StyledProperty<bool> EnableBackProperty =
        AvaloniaProperty.Register<ContentArea, bool>(nameof(EnableBack));

    public static readonly StyledProperty<bool> EnableCancelProperty =
        AvaloniaProperty.Register<ContentArea, bool>(nameof(EnableCancel));

    public static readonly StyledProperty<bool> EnableNextProperty =
        AvaloniaProperty.Register<ContentArea, bool>(nameof(EnableNext));

    public static readonly StyledProperty<object> CancelContentProperty =
        AvaloniaProperty.Register<ContentArea, object>(nameof(CancelContent), "Cancel");

    public static readonly StyledProperty<object> NextContentProperty =
        AvaloniaProperty.Register<ContentArea, object>(nameof(NextContent), "Next");

    public bool EnableBack
    {
        get => GetValue(EnableBackProperty);
        set => SetValue(EnableBackProperty, value);
    }

    public bool EnableCancel
    {
        get => GetValue(EnableCancelProperty);
        set => SetValue(EnableCancelProperty, value);
    }

    public bool EnableNext
    {
        get => GetValue(EnableNextProperty);
        set => SetValue(EnableNextProperty, value);
    }

    public object CancelContent
    {
        get => GetValue(CancelContentProperty);
        set => SetValue(CancelContentProperty, value);
    }

    public object NextContent
    {
        get => GetValue(NextContentProperty);
        set => SetValue(NextContentProperty, value);
    }
}
