using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace DataSettingsSample.ViewModels
{
    public class ListViewModel : ReactiveObject
    {
        private readonly SourceList<ItemViewModel> _sourceList = new();
        private readonly ReadOnlyObservableCollection<ItemViewModel> _items;
        private readonly ObservableAsPropertyHelper<bool> _isLoading;

        public ListViewModel(IList<double> values) : this()
        {
            _sourceList.Edit(innerList =>
            {
                innerList.Clear();
                innerList.AddRange(values.Select(s => new ItemViewModel() { Name = $"{s}" }));
            });
        }

        public ListViewModel()
        {
            _sourceList.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _items)
                .Subscribe();

            Load = ReactiveCommand.CreateFromTask(async () => await Task.Delay(TimeSpan.FromSeconds(2)), outputScheduler: RxApp.MainThreadScheduler);

            _isLoading = Load.IsExecuting
                  .ObserveOn(RxApp.MainThreadScheduler)
                  .ToProperty(this, x => x.IsLoading);
        }

        public ReactiveCommand<Unit, Unit> Load { get; }

        public ReadOnlyObservableCollection<ItemViewModel> Items => _items;

        public bool IsLoading => _isLoading.Value;
    }
}
