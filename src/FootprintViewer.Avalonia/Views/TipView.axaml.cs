using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace FootprintViewer.Avalonia.Views
{
    public partial class TipView : ReactiveUserControl<DrawingTip>
    {
        public TipView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.TextDirty, v => v.Title.Text, _ => ConvertToTitle(ViewModel)).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.TitleDirty, v => v.Text.Text, _ => ConvertToText(ViewModel)).DisposeWith(disposables);
            });
        }

        private static string ConvertToTitle(DrawingTip? tip)
        {
            if (tip == null)
            {
                return string.Empty;
            }

            var target = tip.Target;
            var mode = tip.Mode;
            var value = tip.Value;

            if (mode is TipMode.Init)
            {
                return string.Empty;
            }
            else if (mode is TipMode.BeginCreating)
            {
                return string.Empty;
            }
            else if (mode is TipMode.HoverCreating)
            {
                if (target is TipTarget.Rectangle || target is TipTarget.Circle)
                {
                    return $"{Properties.Resources.Area}: {value}";
                }
                else if (target is TipTarget.Route)
                {
                    return $"{Properties.Resources.Distance}: {value}";
                }
            }
            else if (mode is TipMode.Creating)
            {
                if (target is TipTarget.Polygon)
                {
                    return $"{Properties.Resources.Area}: {value}";
                }
            }

            return "error: title tip not find";
        }

        private static string ConvertToText(DrawingTip? tip)
        {
            if (tip == null)
            {
                return string.Empty;
            }

            var target = tip.Target;
            var mode = tip.Mode;

            if (mode is TipMode.Init)
            {
                if (target is TipTarget.Point)
                {
                    return Properties.Resources.TipInitPoint;
                }
                else if (target is TipTarget.Route)
                {
                    return Properties.Resources.TipInitRoute;
                }
                else if (target is TipTarget.Rectangle)
                {
                    return Properties.Resources.TipInitRectangle;
                }
                else if (target is TipTarget.Circle)
                {
                    return Properties.Resources.TipInitCircle;
                }
                else if (target is TipTarget.Polygon)
                {
                    return Properties.Resources.TipInitPolygon;
                }
            }
            else if (mode is TipMode.BeginCreating)
            {
                if (target is TipTarget.Polygon)
                {
                    return Properties.Resources.TipBeginCreatingPolygon;
                }
            }
            else if (mode is TipMode.HoverCreating)
            {
                if (target is TipTarget.Rectangle)
                {
                    return Properties.Resources.TipBeginCreatingRectangle;
                }
                else if (target is TipTarget.Circle)
                {
                    return Properties.Resources.TipBeginCreatingCircle;
                }
                else if (target is TipTarget.Route)
                {
                    return string.Empty;
                }
            }
            else if (mode is TipMode.Creating)
            {
                if (target is TipTarget.Polygon)
                {
                    return Properties.Resources.TipCreatingPolygon;
                }
            }

            return "error: text tip not find";
        }
    }
}
