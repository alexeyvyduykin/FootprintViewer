using FootprintViewer.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public abstract class BaseViewerList<T> : ReactiveObject, IViewerList<T> where T : IViewerItem
    {
        private readonly ObservableAsPropertyHelper<List<T>> _items;
        private readonly ObservableAsPropertyHelper<bool> _isLoading;
        private T? _prevSelectedItem;

        public BaseViewerList()
        {
            Loading = ReactiveCommand.CreateFromTask<IFilter<T>?, List<T>>(LoadingAsync);

            Select = ReactiveCommand.Create<T, T>(s => s);

            Unselect = ReactiveCommand.Create<T, T>(s => s);

            MouseOverEnter = ReactiveCommand.Create<T, T>(s => s);

            MouseOverLeave = ReactiveCommand.Create(() => { });

            Remove = ReactiveCommand.CreateFromTask<T?>(RemoveAsync);

            Add = ReactiveCommand.CreateFromTask<T?>(AddAsync);

            _items = Loading.ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, x => x.Items);

            _isLoading = Loading.IsExecuting.ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, x => x.IsLoading);
        }

        protected abstract Task<List<T>> LoadingAsync(IFilter<T>? filter = null);

        public ReactiveCommand<IFilter<T>?, List<T>> Loading { get; }

        public ReactiveCommand<T, T> Select { get; }

        public ReactiveCommand<T, T> Unselect { get; }

        public ReactiveCommand<T, T> MouseOverEnter { get; }

        public ReactiveCommand<Unit, Unit> MouseOverLeave { get; }

        public ReactiveCommand<T?, Unit> Add { get; }

        public ReactiveCommand<T?, Unit> Remove { get; }

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

        protected abstract Task AddAsync(T? value);

        protected abstract Task RemoveAsync(T? value);

        public List<T> Items => _items.Value;

        public bool IsLoading => _isLoading.Value;

        public IObservable<T?> SelectedItemObservable => this.WhenAnyValue(s => s.SelectedItem);

        [Reactive]
        public T? SelectedItem { get; set; }
    }
}
