namespace InteractivitySample.Input.Controller.Core
{
    public class MouseDownEventArgs : MouseEventArgs
    {
        public MouseButton ChangedButton { get; set; }

        public int ClickCount { get; set; }
    }
}