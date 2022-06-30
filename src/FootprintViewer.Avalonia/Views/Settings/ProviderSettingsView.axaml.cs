using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace FootprintViewer.Avalonia.Views.Settings
{
    public partial class ProviderSettingsView : ReactiveUserControl<ProviderViewModel>
    {
        public ProviderSettingsView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {

            });
        }
    }
}
