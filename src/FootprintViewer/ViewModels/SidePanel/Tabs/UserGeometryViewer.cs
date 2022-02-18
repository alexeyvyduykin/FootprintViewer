using FootprintViewer.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public class UserGeometryViewer : SidePanelTab
    {
        private readonly UserGeometryProvider _provider;      
        private readonly ObservableAsPropertyHelper<List<UserGeometryInfo>> _userGeometries;
        private readonly ObservableAsPropertyHelper<bool> _isLoading;

        public UserGeometryViewer(IReadonlyDependencyResolver dependencyResolver)
        {
            _provider = dependencyResolver.GetExistingService<UserGeometryProvider>();

            Remove = ReactiveCommand.CreateFromTask<UserGeometryInfo?>(RemoveAsync);

            Loading = ReactiveCommand.CreateFromTask(LoadingAsync);

            Add = ReactiveCommand.CreateFromTask<UserGeometryInfo?>(AddAsync);

            Title = "Пользовательская геометрия";

            Name = "UserGeometryViewer";

            _provider.Update.Select(_ => Unit.Default).InvokeCommand(Loading);

            _userGeometries = Loading.ToProperty(this, x => x.UserGeometries, scheduler: RxApp.MainThreadScheduler);

            _isLoading = Loading.IsExecuting.ToProperty(this, x => x.IsLoading, scheduler: RxApp.MainThreadScheduler);

            this.WhenAnyValue(s => s.IsActive).Where(s => s == true).Select(_ => Unit.Default).InvokeCommand(Loading);
        }

        public ReactiveCommand<UserGeometryInfo?, Unit> Add { get; }

        public ReactiveCommand<UserGeometryInfo?, Unit> Remove { get; }

        private async Task<List<UserGeometryInfo>> LoadingAsync()
        {
            return await _provider.GetUserGeometryInfosAsync();
        }

        private async Task AddAsync(UserGeometryInfo? geometry)
        {
            if (geometry != null)
            {
                await _provider.AddAsync(geometry.Geometry);
            }
        }

        private async Task RemoveAsync(UserGeometryInfo? geometry)
        {
            if (geometry != null)
            {
                await _provider.RemoveAsync(geometry.Geometry);
            }
        }

        public ReactiveCommand<Unit, List<UserGeometryInfo>> Loading { get; }

        public List<UserGeometryInfo> UserGeometries => _userGeometries.Value;

        [Reactive]
        public UserGeometryInfo? SelectedUserGeometry { get; set; }

        public bool IsLoading => _isLoading.Value;
    }
}
