using FootprintViewer.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public class UserGeometryViewer : SidePanelTab
    {
        private readonly UserGeometryProvider _provider;
        private readonly Random _random = new Random();
        private readonly ObservableAsPropertyHelper<List<UserGeometryInfo>> _userGeometries;
        private readonly ObservableAsPropertyHelper<bool> _canRemove;

        public UserGeometryViewer(IReadonlyDependencyResolver dependencyResolver)
        {
            _provider = dependencyResolver.GetExistingService<UserGeometryProvider>();

            _provider.Update.Subscribe(_ => InvalidateData());

            //Create = ReactiveCommand.Create(CreateImpl);
            Create = ReactiveCommand.CreateFromTask(CreateImpl);

            Remove = ReactiveCommand.Create<UserGeometryInfo?>(RemoveImpl);

            Update = ReactiveCommand.Create(InvalidateData);

            LoadUserGeometries = ReactiveCommand.CreateFromTask(_provider.LoadUserGeometriesAsync/* LoadUsersAsync*/);

            _userGeometries = LoadUserGeometries.ToProperty(this, x => x.UserGeometries, scheduler: RxApp.MainThreadScheduler);

            LoadUserGeometries.IsExecuting.Subscribe(can => Can = !can);

            _canRemove = this.WhenAnyValue(x => x.SelectedUserGeometry).Select((g) => g != null).ToProperty(this, x => x.CanRemove);

            //   _canRemove = this.WhenAnyValue(s => s.SelectedUserGeometry).Select(g => g != null).ToProperty(out _canRemove);

            InvalidateData();
        }

        public ReactiveCommand<Unit, Unit> Create { get; }

        public ReactiveCommand<UserGeometryInfo?, Unit> Remove { get; }

        public ReactiveCommand<Unit, Unit> Update { get; }

        [Reactive]
        public bool Can { get; set; }

        public bool CanRemove => _canRemove.Value;

        private async Task CreateImpl()
        {
            await _provider.AddAsync(CreateRandomUserGeometry());
        }

        private UserGeometry CreateRandomUserGeometry()
        {
            string[] names = new[] { "Point", "Rectangle", "Polygon", "Circle" };
            UserGeometryType[] types = new[] { UserGeometryType.Point, UserGeometryType.Rectangle, UserGeometryType.Polygon, UserGeometryType.Circle };

            var index = _random.Next(0, 4);

            var id = Guid.NewGuid();

            return new UserGeometry()
            {
                Name = $"{names[index]}_{id}",
                Type = types[index],
            };
        }

        private void RemoveImpl(UserGeometryInfo? geometry)
        {
            if (geometry != null)
            {
                _provider.Remove(geometry.Geometry);
            }
        }

        private void InvalidateData()
        {
            LoadUserGeometries.Execute().Subscribe();
        }

        public ReactiveCommand<Unit, List<UserGeometryInfo>> LoadUserGeometries { get; }

        public List<UserGeometryInfo> UserGeometries => _userGeometries.Value;

        [Reactive]
        public UserGeometryInfo? SelectedUserGeometry { get; set; }
    }
}
