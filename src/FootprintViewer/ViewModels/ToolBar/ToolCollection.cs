using FootprintViewer.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels
{
    public class ToolCollection : ReactiveObject, IToolCollection
    {
        private readonly IList<IToolCheck> _items;
        private IToolCheck? _first;

        public ToolCollection()
        {
            _items = new List<IToolCheck>();

            Items = new ObservableCollection<IToolCheck>();

            Open = ReactiveCommand.Create(OpenImpl);

            Close = ReactiveCommand.Create(CloseImpl);
        }

        public IEnumerable<IToolCheck> GetItems() => _items;

        public ReactiveCommand<Unit, Unit> Open { get; }

        public ReactiveCommand<Unit, Unit> Close { get; }

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
}
