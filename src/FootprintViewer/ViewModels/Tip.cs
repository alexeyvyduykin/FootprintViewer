using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace FootprintViewer.ViewModels
{
    public interface ITip
    {
        double X { get; set; }

        double Y { get; set; }

        bool IsVisible { get; set; }
    }

    public class TipBase : ReactiveObject, ITip
    {
        [Reactive]
        public double X { get; set; }

        [Reactive]
        public double Y { get; set; }

        [Reactive]
        public bool IsVisible { get; set; }
    }

    public class Tip : TipBase
    {
        [Reactive]
        public string? Title { get; set; }

        [Reactive]
        public string? Text { get; set; }
    }
}
