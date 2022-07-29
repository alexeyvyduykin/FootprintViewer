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

            this.WhenAnyValue(s => s.Updater)
                .Where(s => s != null)
                .Throttle(s => s!.ThrottleDuration!)
                .Select(s => s!.Filter)
                .InvokeCommand(Loading);
        }

        protected abstract Task<List<T>> LoadingAsync(IFilter<T>? filter = null);

        public void FiringUpdate(string[]? names, double seconds)
        {
            var filter = new NameFilter<T>(names);
            FiringUpdate(filter, seconds);
        }

        public void FiringUpdate(IFilter<T>? filter, double seconds)
        {
            Updater = new ViewerListUpdater()
            {
                Filter = filter,
                ThrottleDuration = Observable.Return(Unit.Default).Delay(TimeSpan.FromSeconds(seconds)),
            };
        }

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

        [Reactive]
        private ViewerListUpdater? Updater { get; set; }

        private class ViewerListUpdater
        {
            public IFilter<T>? Filter { get; set; }

            public IObservable<Unit>? ThrottleDuration { get; set; }
        }
    }

    public class NameFilter<T> : BaseFilterViewModel<T> where T : IViewerItem
    {
        private readonly string[]? _names;

        public NameFilter(string[]? names) : base()
        {
            _names = names;
        }

        public override string[]? Names => _names;

        public override IObservable<Func<T, bool>> FilterObservable => throw new NotImplementedException();

        public override bool Filtering(T value)
        {
            return (_names == null) || _names.Contains(value.Name);
        }
    }
}
