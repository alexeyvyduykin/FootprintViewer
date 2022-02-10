using FootprintViewer.Data.Sources;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace FootprintViewer.Data
{
    public class UserGeometryProvider : BaseProvider<IUserGeometryDataSource>
    {
        public UserGeometryProvider()
        {
            Update = ReactiveCommand.Create(() => { });

            Loading = ReactiveCommand.CreateFromTask(LoadUsersAsync);
        }

        public ReactiveCommand<Unit, List<UserGeometry>> Loading { get; }

        public async Task AddAsync(UserGeometry geometry)
        {
            foreach (var source in Sources)
            {
                await source.AddAsync(geometry);
            }

            Update.Execute().Subscribe();
        }

        public void Remove(UserGeometry geometry)
        {
            foreach (var source in Sources)
            {
                source.Remove(geometry);
            }

            Update.Execute().Subscribe();
        }

        public async Task<List<UserGeometry>> LoadUsersAsync()
        {
            return await Sources.First().GetUserGeometriesAsync();

            //List<UserGeometry> list = new List<UserGeometry>();

            //foreach (var source in Sources)
            //{
            //    list.AddRange(await source.GetUserGeometriesAsync());
            //}

            //return list;
        }

        public IEnumerable<UserGeometry> LoadUsers()
        {
            var list = new List<UserGeometry>();

            foreach (var source in Sources)
            {
                list.AddRange(source.GetUserGeometriesAsync().Result);
            }

            return list;
        }

        public async Task<List<UserGeometryInfo>> LoadUserGeometriesAsync()
        {
            return await Task.Run(() =>
            {
                var list = new List<UserGeometryInfo>();

                foreach (var source in Sources)
                {
                    list.AddRange(source.GetUserGeometriesAsync().Result.Select(s => new UserGeometryInfo(s)));
                }

                return list;
            });
        }

        public ReactiveCommand<Unit, Unit> Update { get; }
    }
}
