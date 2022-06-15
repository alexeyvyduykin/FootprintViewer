using Avalonia.ReactiveUI;
using DialogHost;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Disposables;

namespace FootprintViewer.Avalonia.Views.Settings
{
    public partial class ProviderSettingsView : ReactiveUserControl<ProviderSettings>
    {
        public ProviderSettingsView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                foreach (var item in ViewModel!.AvailableSources)
                {
                    ((ReactiveCommand<Unit, ISourceInfo>)item.Build).Subscribe(s => AddSourceButton.Flyout?.Hide()).DisposeWith(disposables);

                    ((ReactiveCommand<Unit, ISourceInfo>)item.Build).Subscribe(s => DialogHost.DialogHost.Show(s, "MainDialogHost", OnDialogClosing)).DisposeWith(disposables);
                }
            });
        }

        private void OnDialogClosing(object? sender, DialogClosingEventArgs e)
        {
            if (e.Parameter != null && e.Parameter is ISourceInfo sourceInfo)
            {
                ViewModel!.AddSource(sourceInfo);
            }
        }
    }
}
