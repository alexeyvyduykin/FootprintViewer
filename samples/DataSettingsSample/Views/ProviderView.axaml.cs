using Avalonia.Controls.Mixins;
using Avalonia.ReactiveUI;
using DataSettingsSample.ViewModels;
using ReactiveUI;
using System;
using System.Reactive.Linq;

namespace DataSettingsSample.Views
{
    public partial class ProviderView : ReactiveUserControl<ProviderViewModel>
    {
        public ProviderView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                ListBoxSources.WhenAnyValue(s => s.SelectedItem).Subscribe(_ => AddSourceButton.Flyout?.Hide()).DisposeWith(disposables);
            });
        }
    }
}
