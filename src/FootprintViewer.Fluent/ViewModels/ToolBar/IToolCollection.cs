using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FootprintViewer.UI.ViewModels.ToolBar;

public interface IToolCollection : IToolItem
{
    void AddItem(IToolCheck item);

    ICommand Open { get; }

    ICommand Close { get; }

    IEnumerable<IToolCheck> GetItems();

    ReadOnlyObservableCollection<IToolCheck> Items { get; }
}