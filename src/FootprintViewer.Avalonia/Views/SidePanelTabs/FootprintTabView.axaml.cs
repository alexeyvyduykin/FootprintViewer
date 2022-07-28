using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs
{
    public partial class FootprintTabView : ReactiveUserControl<FootprintTab>
    {
        public FootprintTabView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                // ToggleButton
                SearchToggleButton.Flyout?.Events().Closing.Subscribe(_ => SearchToggleButton.IsChecked = false).DisposeWith(disposables);
            });
        }
    }
}
