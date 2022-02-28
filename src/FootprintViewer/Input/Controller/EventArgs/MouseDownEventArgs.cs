namespace FootprintViewer.Input
{
    public class MouseDownEventArgs : MouseEventArgs
    {
        public MouseButton ChangedButton { get; set; }

        public int ClickCount { get; set; }
    }
}