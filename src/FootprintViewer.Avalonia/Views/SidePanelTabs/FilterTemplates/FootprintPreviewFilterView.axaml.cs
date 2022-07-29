using Avalonia;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System;
using System.Reactive.Disposables;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs.FilterTemplates
{
    public partial class FootprintPreviewFilterView : ReactiveUserControl<FootprintPreviewTabFilter>
    {
        public FootprintPreviewFilterView()
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
