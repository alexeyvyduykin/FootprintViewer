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
                LayerSelectorButton.Flyout?.Events().Closing.Subscribe(_ => LayerSelectorButton.IsChecked = false).DisposeWith(disposables);

                ViewModel?.MapBackgroundList.WhenAnyValue(s => s.SelectedWorldMap).Subscribe(_ => LayerSelectorButton.Flyout?.Hide()).DisposeWith(disposables);
            });
        }
    }
}
