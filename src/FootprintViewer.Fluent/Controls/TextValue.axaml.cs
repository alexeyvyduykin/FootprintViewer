using Avalonia;
using Avalonia.Controls;

namespace FootprintViewer.Fluent.Controls
{
    public class TextValue : ContentControl
    {
        public static readonly StyledProperty<string> TextProperty =
            AvaloniaProperty.Register<TextValue, string>(nameof(Text));

        public string Text
        {
            get => GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
    }
}
