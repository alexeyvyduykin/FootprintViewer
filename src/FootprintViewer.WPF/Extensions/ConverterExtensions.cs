using System.Windows;
using System.Windows.Input;
using input = FootprintViewer.Input;
using M = Mapsui.Geometries;

namespace FootprintViewer.WPF
{
    public static class ConverterExtensions
    {
        public static M.Point ToScreenPoint(this Point pt)
        {
            return new M.Point(pt.X, pt.Y);
        }

        public static input.MouseButton Convert(this System.Windows.Input.MouseButton mouseButton)
        {
            switch (mouseButton)
            {
                case System.Windows.Input.MouseButton.Left:
                    return input.MouseButton.Left;
                case System.Windows.Input.MouseButton.Middle:
                    return input.MouseButton.Middle;
                case System.Windows.Input.MouseButton.Right:
                    return input.MouseButton.Right;
                default:
                    return input.MouseButton.None;
            }
        }

        public static input.MouseWheelEventArgs ToMouseWheelEventArgs(this System.Windows.Input.MouseWheelEventArgs e, IInputElement relativeTo)
        {
            return new input.MouseWheelEventArgs
            {
                Position = e.GetPosition(relativeTo).ToScreenPoint(),
                //ModifierKeys = Keyboard.Instance.GetModifierKeys(),
                Delta = e.Delta/*e.Delta.Y + e.Delta.X*/ * 120
            };
        }

        public static input.MouseEventArgs ToMouseEventArgs(this System.Windows.Input.MouseEventArgs e, IInputElement relativeTo)
        {
            return new input.MouseEventArgs
            {
                Position = e.GetPosition(relativeTo).ToScreenPoint(),
                //ModifierKeys = e.KeyModifiers.ToModifierKeys()
            };
        }

        public static input.MouseDownEventArgs ToMouseDownEventArgs(this MouseButtonEventArgs e, IInputElement relativeTo)
        {
            return new input.MouseDownEventArgs
            {
                ChangedButton = e.ChangedButton.Convert(),
                ClickCount = e.ClickCount,
                Position = e.GetPosition(relativeTo).ToScreenPoint(),
                //ModifierKeys = e.KeyModifiers.ToModifierKeys()
            };
        }

        public static input.MouseEventArgs ToMouseReleasedEventArgs(this MouseButtonEventArgs e, IInputElement relativeTo)
        {
            return new input.MouseEventArgs
            {
                Position = e.GetPosition(relativeTo).ToScreenPoint(),
                //ModifierKeys = e.KeyModifiers.ToModifierKeys()
            };
        }
    }
}
