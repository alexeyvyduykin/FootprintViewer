using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;

namespace UserGeometriesDatabaseSample.Data
{
    public class LocalDataSource : ReactiveObject, IDataSource
    {
        private readonly List<UserGeometry> _userGeometries;

        public LocalDataSource()
        {
            _userGeometries = new List<UserGeometry>();

            Update = ReactiveCommand.Create(() => { });
        }

        public ReactiveCommand<Unit, Unit> Update { get; }

        public void Add(UserGeometry geometry)
        {
            _userGeometries.Add(geometry);

            Update.Execute().Subscribe();
        }

        public void Remove(UserGeometry geometry)
        {
            _userGeometries.Remove(geometry);

            Update.Execute().Subscribe();
        }

        public async Task<List<UserGeometry>> LoadUsersAsync()
        {
            //await Task.Delay(2000);

            return await Task.Run(() =>
            {
                return new List<UserGeometry>(_userGeometries);
            });
        }
    }
}
