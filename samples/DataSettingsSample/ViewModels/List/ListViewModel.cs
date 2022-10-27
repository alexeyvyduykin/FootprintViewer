using DataSettingsSample.Data;
using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        private readonly IList<double>? _values;
        private readonly Repository? _repository;
        private readonly string? _key;
        private readonly Func<object, IEnumerable<ItemViewModel>>? _converter;

        public ListViewModel(string key, Repository repository, Func<object, IEnumerable<ItemViewModel>> converter) : this()
        {
            _key = key;
            _repository = repository;
            _converter = converter;
        }

        public ListViewModel(IList<double> values) : this()
        {
            _values = values;

        }

        public ListViewModel()
        {
            _sourceList.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _items)
                .Subscribe();

            Load = ReactiveCommand.CreateFromTask(LoadAsyncImpl, outputScheduler: RxApp.MainThreadScheduler);

            _isLoading = Load.IsExecuting
                  .ObserveOn(RxApp.MainThreadScheduler)
                  .ToProperty(this, x => x.IsLoading);
        }

        private async Task LoadAsyncImpl()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));

            if (_values != null)
            {
                _sourceList.Edit(innerList =>
                {
                    innerList.Clear();
                    innerList.AddRange(_values.Select(s => new ItemViewModel() { Name = $"{s}" }));
                });
            }
            else if (_repository != null && _key != null && _converter != null)
            {
                var list = await _repository.GetDataAsync<object>(_key);
                var res = list.SelectMany(s => _converter.Invoke(s));

                Debug.WriteLine($"For Key={_key}: data is loading.");

                _sourceList.Edit(innerList =>
                {
                    innerList.Clear();
                    innerList.AddRange(res);
                });
            }
        }

        public ReactiveCommand<Unit, Unit> Load { get; }

        public ReadOnlyObservableCollection<ItemViewModel> Items => _items;

        public bool IsLoading => _isLoading.Value;
    }
}
