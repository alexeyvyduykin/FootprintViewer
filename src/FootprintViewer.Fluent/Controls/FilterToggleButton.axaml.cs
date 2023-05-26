using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using System.Windows.Input;

namespace FootprintViewer.UI.Controls
{
    public partial class FilterToggleButton : ToggleButton
    {
        private ICommand? _resetCommand;

        public static readonly StyledProperty<string> TitleProperty =
            AvaloniaProperty.Register<FilterToggleButton, string>(nameof(Title));

        public string Title
        {
            get => GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly StyledProperty<Flyout?> FilterProperty =
            AvaloniaProperty.Register<FilterToggleButton, Flyout?>(nameof(Filter));

        public Flyout? Filter
        {
            get => GetValue(FilterProperty);
            set => SetValue(FilterProperty, value);
        }

        public static readonly StyledProperty<FlyoutPlacementMode> PlacementProperty =
            AvaloniaProperty.Register<FilterToggleButton, FlyoutPlacementMode>(nameof(Placement), FlyoutPlacementMode.LeftEdgeAlignedTop);

        public FlyoutPlacementMode Placement
        {
            get => GetValue(PlacementProperty);
            set => SetValue(PlacementProperty, value);
        }

        public static readonly StyledProperty<bool> IsDirtyProperty =
            AvaloniaProperty.Register<FilterToggleButton, bool>(nameof(IsDirty), false);

        public bool IsDirty
        {
            get => GetValue(IsDirtyProperty);
            set => SetValue(IsDirtyProperty, value);
        }

        public static readonly DirectProperty<FilterToggleButton, ICommand?> ResetCommandProperty =
            AvaloniaProperty.RegisterDirect<FilterToggleButton, ICommand?>(nameof(ResetCommand), s => s.ResetCommand, (s, command) => s.ResetCommand = command, enableDataValidation: true);

        public ICommand? ResetCommand
        {
            get { return _resetCommand; }
            set { SetAndRaise(ResetCommandProperty, ref _resetCommand, value); }
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            ClickMode = ClickMode.Press;

            Flyout = Filter;

            base.OnApplyTemplate(e);
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == FilterProperty)
            {
                if (Filter is { })
                {
                    Filter.Placement = Placement;
                    // TODO: ???                 
                    Filter.Closed += Flyout_Closed;
                }
            }
            else if (e.Property == PlacementProperty)
            {
                if (Filter is { })
                {
                    Filter.Placement = Placement;
                }
            }
        }

        private void Flyout_Closed(object? sender, System.EventArgs e)
        {
            IsChecked = false;
        }
    }
}
