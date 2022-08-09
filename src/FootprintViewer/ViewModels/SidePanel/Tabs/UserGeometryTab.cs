using DynamicData;
using FootprintViewer.Data;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public class UserGeometryTab : SidePanelTab
    {
        private readonly IEditableProvider<UserGeometry> _provider;
        private readonly SourceList<UserGeometryViewModel> _userGeometries = new();
        private readonly ReadOnlyObservableCollection<UserGeometryViewModel> _items;
        private readonly ObservableAsPropertyHelper<bool> _isLoading;

        public UserGeometryTab(IReadonlyDependencyResolver dependencyResolver)
        {
            _provider = dependencyResolver.GetExistingService<IEditableProvider<UserGeometry>>();

            Title = "Пользовательская геометрия";

            _userGeometries
                .Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _items)
                .Subscribe();

            Loading = ReactiveCommand.CreateFromTask(LoadingImpl);

            Delay = ReactiveCommand.CreateFromTask(() => Task.Delay(TimeSpan.FromSeconds(1.0)));

            Remove = ReactiveCommand.CreateFromTask<UserGeometryViewModel?>(RemoveAsync);

            _isLoading = Delay.IsExecuting
                  .ObserveOn(RxApp.MainThreadScheduler)
                  .ToProperty(this, x => x.IsLoading);

            _provider.Update.Select(_ => Unit.Default).InvokeCommand(Loading);

            this.WhenAnyValue(s => s.IsActive)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Where(active => active == true)
                .Take(1)
                .Select(_ => Unit.Default)
                .InvokeCommand(Loading);

            this.WhenAnyValue(s => s.IsActive)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Where(active => active == true)
                .Select(_ => Unit.Default)
                .InvokeCommand(Delay);
        }

        public ReactiveCommand<Unit, Unit> Loading { get; }

        private ReactiveCommand<Unit, Unit> Delay { get; }

        public ReactiveCommand<UserGeometryViewModel?, Unit> Remove { get; }

        public bool IsLoading => _isLoading.Value;

        private async Task LoadingImpl()
        {
            var list = await _provider.GetValuesAsync(null, s => new UserGeometryViewModel(s));

            _userGeometries.Edit(innerList =>
            {
                innerList.Clear();
                innerList.AddRange(list);
            });
        }

        private async Task RemoveAsync(UserGeometryViewModel? value)
        {
            if (value != null && _provider is IEditableProvider<UserGeometry> editableProvider)
            {
                await editableProvider.RemoveAsync(value.Model);
            }
        }

        public async Task<List<UserGeometryViewModel>> GetUserGeometryViewModelsAsync(string name)
        {
            return await _provider.GetValuesAsync<UserGeometryViewModel>(
                new NameFilter<UserGeometryViewModel>(new[] { name }), s => new UserGeometryViewModel(s));
        }

        public ReadOnlyObservableCollection<UserGeometryViewModel> Items => _items;
    }
}
