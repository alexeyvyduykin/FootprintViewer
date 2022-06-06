using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace FootprintViewer.Avalonia.Views.ToolBar
{
    public partial class CustomToolBarView : ReactiveUserControl<CustomToolBar>
    {
        public CustomToolBarView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                // MapBackgroundSelectorButton
                MapBackgroundSelectorButton.Flyout?.Events().Closing.Subscribe(_ => MapBackgroundSelectorButton.IsChecked = false).DisposeWith(disposables);

                ViewModel?.MapBackgroundList.WhenAnyValue(s => s.SelectedMapBackground).Subscribe(_ => MapBackgroundSelectorButton.Flyout?.Hide()).DisposeWith(disposables);

                // MapLayerSelectorButton
                MapLayerSelectorButton.Flyout?.Events().Closing.Subscribe(_ => MapLayerSelectorButton.IsChecked = false).DisposeWith(disposables);
            });
        }
    }
}
