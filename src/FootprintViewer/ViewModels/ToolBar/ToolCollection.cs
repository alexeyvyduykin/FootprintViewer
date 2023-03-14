using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Windows.Input;

namespace FootprintViewer.ViewModels.ToolBar;

public class ToolCollection : ViewModelBase, IToolCollection
{
    private readonly IList<IToolCheck> _items;
    private IToolCheck? _first;

    public ToolCollection()
    {
        _items = new List<IToolCheck>();

        Items = new ObservableCollection<IToolCheck>();

        Open = ReactiveCommand.Create<ToolCollection>(_ => OpenImpl());

        Close = ReactiveCommand.Create(CloseImpl);
    }

    public IEnumerable<IToolCheck> GetItems() => _items;

    public ICommand Open { get; }

    public ICommand Close { get; }

    public void OpenImpl()
    {
        Items = new ObservableCollection<IToolCheck>(_items);
    }

    public void CloseImpl()
    {
        if (_first != null)
        {
            Items = new ObservableCollection<IToolCheck>(new[] { _first });
        }
        else
        {
            Items = new ObservableCollection<IToolCheck>();
        }
    }

    public void AddItem(IToolCheck item)
    {
        _items.Add(item);

        item.Activate.Subscribe(ActivateChanged);

        item.Deactivate.Subscribe(_ => CloseImpl());

        if (_items.Count == 1)
        {
            _first = item;

            Items = new ObservableCollection<IToolCheck>(new[] { item });
        }
    }

    private void ActivateChanged(IToolCheck tool)
    {
        _first = tool;

        _items.Remove(tool);

        _items.Insert(0, tool);

        CloseImpl();
    }

    [Reactive]
    public ObservableCollection<IToolCheck> Items { get; set; }
}

public class ToolCollection2 : ViewModelBase, IToolCollection2
{
    private readonly IList<IToolCheck2> _items;
    private IToolCheck2? _first;

    public ToolCollection2()
    {
        _items = new List<IToolCheck2>();

        Items = new ObservableCollection<IToolCheck2>();

        Open = ReactiveCommand.Create<ToolCollection2>(_ => OpenImpl());

        Close = ReactiveCommand.Create(CloseImpl);
    }

    public IEnumerable<IToolCheck2> GetItems() => _items;

    public ICommand Open { get; }

    public ICommand Close { get; }

    public void OpenImpl()
    {
        Items = new ObservableCollection<IToolCheck2>(_items);
    }

    public void CloseImpl()
    {
        if (_first != null)
        {
            Items = new ObservableCollection<IToolCheck2>(new[] { _first });
        }
        else
        {
            Items = new ObservableCollection<IToolCheck2>();
        }
    }

    public void AddItem(IToolCheck2 item)
    {
        _items.Add(item);

        //item.Activate.Subscribe(ActivateChanged);

        //item.Deactivate.Subscribe(_ => CloseImpl());

        item.IsCheckObservable.Where(s => s == true).Subscribe(_ => ActivateChanged(item));

        item.IsCheckObservable.Where(s => s == false).Subscribe(_ => CloseImpl());

        if (_items.Count == 1)
        {
            _first = item;

            Items = new ObservableCollection<IToolCheck2>(new[] { item });
        }
    }

    private void ActivateChanged(IToolCheck2 tool)
    {
        _first = tool;

        _items.Remove(tool);

        _items.Insert(0, tool);

        CloseImpl();
    }

    [Reactive]
    public ObservableCollection<IToolCheck2> Items { get; set; }
}
