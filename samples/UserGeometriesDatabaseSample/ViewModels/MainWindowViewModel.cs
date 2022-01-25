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

        public MainWindowViewModel(IReadonlyDependencyResolver dependencyResolver)
        {
            _dataSource = dependencyResolver.GetService<IDataSource>() ?? throw new Exception();

            _dataSource.Update.Subscribe(_ => InvalidateData());

            Create = ReactiveCommand.Create(CreateImpl);

            Update = ReactiveCommand.Create(InvalidateData);

            LoadUsers = ReactiveCommand.CreateFromTask(_dataSource.LoadUsersAsync);

            _users = LoadUsers.ToProperty(this, x => x.UserGeometries, scheduler: RxApp.MainThreadScheduler);

            LoadUsers.IsExecuting.Subscribe(can => Can = !can);

            InvalidateData();
        }

        public ReactiveCommand<Unit, Unit> Create { get; }

        public ReactiveCommand<Unit, Unit> Update { get; }

        [Reactive]
        public bool Can { get; set; }

        private void CreateImpl()
        {
            string[] names = new[] { "Point", "Rectangle", "Polygon", "Circle" };
            UserGeometryType[] types = new[] { UserGeometryType.Point, UserGeometryType.Rectangle, UserGeometryType.Polygon, UserGeometryType.Circle };

            var index = _random.Next(0, 3);

            var id = Guid.NewGuid();

            var g = new UserGeometry()
            {
                Name = $"{names[index]}_{id}",
                Type = types[index],
            };

            _dataSource.Add(g);
        }

        private void InvalidateData()
        {                
            LoadUsers.Execute().Subscribe();
        }

        public ReactiveCommand<Unit, List<UserGeometry>> LoadUsers { get; }


        public List<UserGeometry> UserGeometries => _users.Value;
    }
}
