using ReactiveUI;

namespace FootprintViewer.ViewModels
{
    public enum TipTarget
    {
        Point,
        Route,
        Rectangle,
        Circle,
        Polygon
    }

    public enum TipMode
    {
        Init,
        BeginCreating,
        HoverCreating,
        Creating
    }

    public static class DrawingTips
    {
        public static DrawingTip CreatePointTip() => new(TipTarget.Point);

        public static DrawingTip CreateRouteTip() => new(TipTarget.Route);

        public static DrawingTip CreateRectangleTip() => new(TipTarget.Rectangle);

        public static DrawingTip CreateCircleTip() => new(TipTarget.Circle);

        public static DrawingTip CreatePolygonTip() => new(TipTarget.Polygon);
    }

    public class DrawingTip : Tip
    {
        private object _title = default(bool);
        private object _text = default(bool);

        public DrawingTip(TipTarget target)
        {
            Target = target;
            Mode = TipMode.Init;
            Value = null;

            InvalidateVisual();
        }

        public object TitleDirty => _title;

        public object TextDirty => _text;

        protected void InvalidateVisual()
        {
            this.RaiseAndSetIfChanged(ref _title, !(bool)_title, nameof(TitleDirty));
            this.RaiseAndSetIfChanged(ref _text, !(bool)_text, nameof(TextDirty));
        }

        public TipTarget Target { get; private set; }

        public TipMode Mode { get; private set; }

        public object? Value { get; private set; }

        public void HoverCreating(object? value = null)
        {
            Mode = TipMode.HoverCreating;
            Value = value;

            InvalidateVisual();
        }

        public void BeginCreating(object? value = null)
        {
            Mode = TipMode.BeginCreating;
            Value = value;

            InvalidateVisual();
        }

        public void Creating(object? value = null)
        {
            Mode = TipMode.Creating;
            Value = value;

            InvalidateVisual();
        }
    }
}
