using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using UserGeometriesDatabaseSample.Data;

namespace UserGeometriesDatabaseSample.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IDataSource _dataSource;
        private readonly Random _random = new Random();
        private readonly ObservableAsPropertyHelper<List<UserGeometry>> _users;
        private readonly ObservableAsPropertyHelper<bool> _canRemove;

        public MainWindowViewModel(IReadonlyDependencyResolver dependencyResolver)
        {
            _dataSource = dependencyResolver.GetRequiredService<IDataSource>();

            _dataSource.Update.Subscribe(_ => InvalidateData());

            Create = ReactiveCommand.Create(CreateImpl);

            Remove = ReactiveCommand.Create<UserGeometry?>(RemoveImpl);

            Update = ReactiveCommand.Create(InvalidateData);

            LoadUsers = ReactiveCommand.CreateFromTask(_dataSource.LoadUsersAsync);

            _users = LoadUsers.ToProperty(this, x => x.UserGeometries, scheduler: RxApp.MainThreadScheduler);

            LoadUsers.IsExecuting.Subscribe(can => Can = !can);

            _canRemove = this.WhenAnyValue(x => x.SelectedUserGeometry).Select((g) => g != null).ToProperty(this, x => x.CanRemove);

            //   _canRemove = this.WhenAnyValue(s => s.SelectedUserGeometry).Select(g => g != null).ToProperty(out _canRemove);

            InvalidateData();
        }

        public ReactiveCommand<Unit, Unit> Create { get; }

        public ReactiveCommand<UserGeometry?, Unit> Remove { get; }

        public ReactiveCommand<Unit, Unit> Update { get; }

        [Reactive]
        public bool Can { get; set; }

        public bool CanRemove => _canRemove.Value;

        private void CreateImpl()
        {
            _dataSource.Add(CreateRandomUserGeometry());
        }

        private UserGeometry CreateRandomUserGeometry()
        {
            string[] names = new[] { "Point", "Rectangle", "Polygon", "Circle" };
            UserGeometryType[] types = new[] { UserGeometryType.Point, UserGeometryType.Rectangle, UserGeometryType.Polygon, UserGeometryType.Circle };

            var index = _random.Next(0, 3);

            var id = Guid.NewGuid();

            return new UserGeometry()
            {
                Name = $"{names[index]}_{id}",
                Type = types[index],
            };
        }

        private void RemoveImpl(UserGeometry? geometry)
        {
            if (geometry != null)
            {
                _dataSource.Remove(geometry);
            }
        }

        private void InvalidateData()
        {                
            LoadUsers.Execute().Subscribe();
        }

        public ReactiveCommand<Unit, List<UserGeometry>> LoadUsers { get; }

        public List<UserGeometry> UserGeometries => _users.Value;

        [Reactive]
        public UserGeometry? SelectedUserGeometry { get; set; }
    }
}
