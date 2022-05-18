using Avalonia;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System;
using System.Reactive.Disposables;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs
{
    public partial class SceneSearchFilterView : ReactiveUserControl<SceneSearchFilter>
    {
        public SceneSearchFilterView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.Bind(ViewModel, vm => vm.FromDate, v => v.FromDatePicker.SelectedDate,
                    value => new DateTimeOffset(value),
                    value => ((DateTimeOffset)value!).DateTime).DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.ToDate, v => v.ToDatePicker.SelectedDate,
                    value => new DateTimeOffset(value),
                    value => ((DateTimeOffset)value!).DateTime).DisposeWith(disposables);
            });
        }
    }
}
