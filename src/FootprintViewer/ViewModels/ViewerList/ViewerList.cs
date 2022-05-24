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
    public interface IViewerList<T>
    {
        void FiringUpdate(string[]? names, double seconds = 1.0);

        void FiringUpdate(IFilter<T>? filter, double seconds = 1.0);

        ReactiveCommand<IFilter<T>?, List<T>> Loading { get; }

        ReactiveCommand<T, T> Select { get; }

        ReactiveCommand<T, T> Unselect { get; }

        ReactiveCommand<T, T> MouseOverEnter { get; }

        ReactiveCommand<Unit, Unit> MouseOverLeave { get; }

        ReactiveCommand<T?, Unit> Add { get; }

        ReactiveCommand<T?, Unit> Remove { get; }

        void ClickOnItem(T? item);

        void SelectItem(string name);

        T? GetItem(string name);

        IObservable<T?> SelectedItemObservable { get; }

        bool IsLoading { get; }

        T? SelectedItem { get; set; }

        List<T> Items { get; }
    }

    public interface IViewerItem
    {
        string Name { get; }

        bool IsShowInfo { get; set; }
    }

    public class ViewerList<T> : ReactiveObject, IViewerList<T> where T : IViewerItem
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

        private async Task<List<T>> LoadingAsync(IFilter<T>? filter = null)
        {
            return await _provider.GetValuesAsync(filter);
        }


        public void FiringUpdate(string[]? names, double seconds)
        {
            FiringUpdate(new NameFilter<T>(names), seconds);
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

        private async Task AddAsync(T? value)
        {
            if (value != null && _provider is IEditableProvider<T> editableProvider)
            {
                await editableProvider.AddAsync(value);
            }
        }

        private async Task RemoveAsync(T? value)
        {
            if (value != null && _provider is IEditableProvider<T> editableProvider)
            {
                await editableProvider.RemoveAsync(value);
            }
        }

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
}
