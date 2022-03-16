using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System;
using System.Reactive.Disposables;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs
{
    public partial class SceneSearchFilterView : ReactiveUserControl<SceneSearchFilter>
    {
        private DatePicker FromDatePicker => this.FindControl<DatePicker>("FromDatePicker");

        private DatePicker ToDatePicker => this.FindControl<DatePicker>("ToDatePicker");

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

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
