using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;

namespace FootprintViewer.ViewModels.ToolBar;

public class ToolCollection : ViewModelBase, IToolCollection
{
    private readonly List<IToolCheck> _cacheItems = new();
    private readonly SourceList<IToolCheck> _tools = new();
    private readonly ReadOnlyObservableCollection<IToolCheck> _items;

    public ToolCollection()
    {
        _tools
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _items)
            .DisposeMany()
            .Subscribe();

        Open = ReactiveCommand.Create<ToolCollection>(_ => OpenImpl());

        Close = ReactiveCommand.Create(CloseImpl);
    }

    public IEnumerable<IToolCheck> GetItems() => _items;

    public ICommand Open { get; }

    public ICommand Close { get; }

    public void OpenImpl()
    {
        _tools.Edit(innerList =>
        {
            innerList.Clear();
            innerList.AddRange(_cacheItems);
        });
    }

    public void CloseImpl()
    {
        _tools.Edit(innerList =>
        {
            innerList.Clear();

            var first = _cacheItems.FirstOrDefault();

            if (first != null)
            {
                innerList.Add(first);
            }
        });
    }

    public void AddItem(IToolCheck item)
    {
        _cacheItems.Add(item);

        item.Activate.Subscribe(ActivateChanged);

        item.Deactivate.Subscribe(_ => CloseImpl());

        if (_cacheItems.Count == 1)
        {
            _tools.Edit(innerList =>
            {
                innerList.Clear();
                innerList.AddRange(new[] { item });
            });
        }
    }

    private void ActivateChanged(IToolCheck tool)
    {
        _cacheItems.Remove(tool);

        _cacheItems.Insert(0, tool);

        CloseImpl();
    }

    public ReadOnlyObservableCollection<IToolCheck> Items => _items;
}
