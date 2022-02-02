using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;

namespace FootprintViewer.Models
{
    public interface IToolCollection : IToolItem
    {
        void AddItem(IToolCheck item);

        ReactiveCommand<Unit, Unit> Open { get; }

        ReactiveCommand<Unit, Unit> Close { get; }

        IEnumerable<IToolCheck> GetItems();

        ObservableCollection<IToolCheck> Items { get; }
    }
}
