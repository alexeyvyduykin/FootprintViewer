using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using UserGeometriesDatabaseSample.Data;

namespace UserGeometriesDatabaseSample.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IDataSource _dataSource;

        public MainWindowViewModel(IReadonlyDependencyResolver dependencyResolver)
        {
            _dataSource = dependencyResolver.GetService<IDataSource>() ?? throw new Exception();

            var arr = _dataSource.UserGeometries;

            UserGeometries = new ObservableCollection<UserGeometry>(arr);
        }

        [Reactive]
        public ObservableCollection<UserGeometry> UserGeometries { get; set; }
    }
}
