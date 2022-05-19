using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using FootprintViewer.Models;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace FootprintViewer.Avalonia.Views.ToolBar
{
    public partial class ToolCollectionView : ReactiveUserControl<IToolCollection>
    {
        public ToolCollectionView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                CollectionStackPanel.Events().PointerEnter.Select(args => Unit.Default).InvokeCommand(this, v => v.ViewModel!.Open).DisposeWith(disposables);

                CollectionStackPanel.Events().PointerLeave.Select(args => Unit.Default).InvokeCommand(this, v => v.ViewModel!.Close).DisposeWith(disposables);
            });
        }
    }
}
