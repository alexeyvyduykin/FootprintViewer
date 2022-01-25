using Avalonia;
using NetTopologySuite.Geometries;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using UserGeometriesDatabaseSample.Data;
using UserGeometriesDatabaseSample.ViewModels;

namespace UserGeometriesDatabaseSample.Designer
{
    public class DesignTimeUserGeometry : UserGeometry
    {
        public DesignTimeUserGeometry()
        {
            Name = "Rectangle032";
            Type = UserGeometryType.Rectangle;
            Geometry = new Polygon(new LinearRing(new Coordinate[] { }));
        }
    }

    public class DesignTimeMainWindowViewModel : MainWindowViewModel
    {
        public DesignTimeMainWindowViewModel() : base(new DesignTimeLocator()) { }
    }

    public class DesignTimeLocator : IReadonlyDependencyResolver
    {
        private DesignTimeDataSource? _designTimeDataSource;

        public object? GetService(Type? serviceType, string? contract = null)
        {
            if (serviceType == typeof(IDataSource))
            {
                return _designTimeDataSource ??= new DesignTimeDataSource();
            }

            throw new NotImplementedException();
        }

        public IEnumerable<object> GetServices(Type? serviceType, string? contract = null)
        {
            throw new NotImplementedException();
        }
    }

    public class DesignTimeDataSource : IDataSource
    {
        public DesignTimeDataSource()
        {
            var arr = new UserGeometry[]
            {
                new UserGeometry() { Name="fdfdfds", Type = UserGeometryType.Circle } ,
                new UserGeometry() { Name="sdfjhjhg", Type = UserGeometryType.Rectangle } ,
                new UserGeometry() { Name="fs 454 tftfg", Type = UserGeometryType.Point } ,
            };

            UserGeometries = new List<UserGeometry>(arr);

            Update = ReactiveCommand.Create(() => { });
        }

        private List<UserGeometry> UserGeometries { get; }

        public ReactiveCommand<Unit, Unit> Update { get; }

        public void Add(UserGeometry geometry)
        {
            throw new NotImplementedException();
        }

        public async Task<List<UserGeometry>> LoadUsersAsync()
        {
            return await Task.Run(() => UserGeometries);
        }
    }
}
