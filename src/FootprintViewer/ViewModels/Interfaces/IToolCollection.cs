using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FootprintViewer.ViewModels;

public interface IToolCollection : IToolItem
{
    void AddItem(IToolCheck item);

    ICommand Open { get; }

    ICommand Close { get; }

    IEnumerable<IToolCheck> GetItems();

    ObservableCollection<IToolCheck> Items { get; }
}

public interface IToolCollection2 : IToolItem
{
    void AddItem(IToolCheck2 item);

    ICommand Open { get; }

    ICommand Close { get; }

    IEnumerable<IToolCheck2> GetItems();

    ObservableCollection<IToolCheck2> Items { get; }
}