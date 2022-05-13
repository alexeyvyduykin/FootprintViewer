using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public interface IViewerItem
    {
        string Name { get; }

        bool IsShowInfo { get; set; }
    }

    public interface IProvider<T>
    {
        Task<List<T>> GetValuesAsync();
    }

    public interface IFilter<T>
    {
        bool Filtering(T value);
    }

    public class ViewerList<T> : ReactiveObject where T : IViewerItem
    {
        private readonly ObservableAsPropertyHelper<List<T>> _items;
        private readonly ObservableAsPropertyHelper<bool> _isLoading;
        private T? _prevSelectedItem;
        private readonly IProvider<T> _provider;

        public ViewerList(IProvider<T> provider)
        {
            _provider = provider;

            Loading = ReactiveCommand.CreateFromTask<IFilter<T>?, List<T>>(LoadingAsync);

            Select = ReactiveCommand.Create<T, T>(s => s);

            Unselect = ReactiveCommand.Create<T, T>(s => s);

            _items = Loading.ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, x => x.Items);

            _isLoading = Loading.IsExecuting.ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, x => x.IsLoading);
        }

        private async Task<List<T>> LoadingAsync(IFilter<T>? filter = null)
        {
            if (filter == null)
            {
                return await _provider.GetValuesAsync();
            }
            else
            {
                return await Task.Run(() =>
                {
                    var list = _provider.GetValuesAsync().Result;

                    return list.Where(s => filter.Filtering(s)).ToList();
                });
            }
        }

        public ReactiveCommand<IFilter<T>?, List<T>> Loading { get; }

        public ReactiveCommand<T, T> Select { get; }

        public ReactiveCommand<T, T> Unselect { get; }

        public void ClickOnItem(T? item)
        {
            if (item == null)
            {
                return;
            }

            if (_prevSelectedItem != null && _prevSelectedItem.Name.Equals(item.Name) == false)
            {
                if (_prevSelectedItem.IsShowInfo == true)
                {
                    _prevSelectedItem.IsShowInfo = false;
                }
            }

            item.IsShowInfo = !item.IsShowInfo;

            if (item.IsShowInfo == true)
            {
                Select.Execute(item).Subscribe();
            }
            else
            {
                Unselect.Execute(item).Subscribe();
            }

            _prevSelectedItem = item;
        }

        public void SelectItem(string name)
        {
            var item = Items.Where(s => s.Name.Equals(name)).FirstOrDefault();

            ClickOnItem(item);
        }

        public T? GetItem(string name)
        {
            return Items.Where(s => s.Name.Equals(name)).FirstOrDefault();
        }

        public List<T> Items => _items.Value;

        public bool IsLoading => _isLoading.Value;
    }
}
