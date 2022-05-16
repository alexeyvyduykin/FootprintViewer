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
    public interface IViewerItem
    {
        string Name { get; }

        bool IsShowInfo { get; set; }
    }

    public interface IProvider<T>
    {
        Task<List<T>> GetValuesAsync(IFilter<T>? filter = null);
    }

    public class ViewerList<T> : ReactiveObject where T : IViewerItem
    {
        private readonly ObservableAsPropertyHelper<List<T>> _items;
        private readonly ObservableAsPropertyHelper<bool> _isLoading;
        private T? _prevSelectedItem;
        private readonly IProvider<T> _provider;
        private IFilter<T>? _filter;

        public ViewerList(IProvider<T> provider)
        {
            _provider = provider;

            Loading = ReactiveCommand.CreateFromTask<IFilter<T>?, List<T>>(LoadingAsync);

            Select = ReactiveCommand.Create<T, T>(s => s);

            Unselect = ReactiveCommand.Create<T, T>(s => s);

            MouseOverEnter = ReactiveCommand.Create<T, T>(s => s);

            MouseOverLeave = ReactiveCommand.Create(() => { });

            _items = Loading.ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, x => x.Items);

            this.WhenAnyValue(s => s.Checker).Throttle(TimeSpan.FromSeconds(1)).Select(_ => _filter).InvokeCommand(Loading);

            _isLoading = Loading.IsExecuting.ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, x => x.IsLoading);
        }

        private async Task<List<T>> LoadingAsync(IFilter<T>? filter = null)
        {
            return await _provider.GetValuesAsync(filter);
        }

        public void Update(string[]? names = null)
        {
            _filter = new NameFilter<T>(names);
            Checker = !Checker;
        }

        public ReactiveCommand<IFilter<T>?, List<T>> Loading { get; }

        public ReactiveCommand<T, T> Select { get; }

        public ReactiveCommand<T, T> Unselect { get; }

        public ReactiveCommand<T, T> MouseOverEnter { get; }

        public ReactiveCommand<Unit, Unit> MouseOverLeave { get; }

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

        [Reactive]
        private bool Checker { get; set; }

        public IObservable<T?> SelectedItemObservable => this.WhenAnyValue(s => s.SelectedItem);

        [Reactive]
        public T? SelectedItem { get; set; }
    }
}
