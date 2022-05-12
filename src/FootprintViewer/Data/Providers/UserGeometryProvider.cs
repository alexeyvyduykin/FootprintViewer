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

            Loading = ReactiveCommand.CreateFromTask(GetUserGeometryInfosAsync);
        }

        public ReactiveCommand<Unit, List<UserGeometryInfo>> Loading { get; }

        public ReactiveCommand<Unit, Unit> Update { get; }

        public async Task UpdateGeometry(string key, NetTopologySuite.Geometries.Geometry geometry)
        {
            await Sources.FirstOrDefault()!.UpdateGeometry(key, geometry);
        }

        public async Task AddAsync(UserGeometry geometry)
        {
            foreach (var source in Sources)
            {
                await source.AddAsync(geometry);
            }

            Update.Execute().Subscribe();
        }

        public async Task RemoveAsync(UserGeometry geometry)
        {
            foreach (var source in Sources)
            {
                await source.RemoveAsync(geometry);
            }

            Update.Execute().Subscribe();
        }

        public async Task<List<UserGeometryInfo>> GetUserGeometryInfosAsync()
        {
            return await Sources.FirstOrDefault()!.GetUserGeometryInfosAsync();

            //return await Task.Run(() =>
            //{
            //    var list = new List<UserGeometryInfo>();

            //    foreach (var source in Sources)
            //    {
            //        list.AddRange(source.GetUserGeometriesAsync().Result.Select(s => new UserGeometryInfo(s)));
            //    }

            //    return list;
            //});
        }

        public async Task<List<UserGeometryInfo>> GetUserGeometryInfosAsync(string[] names)
        {
            return await Sources.FirstOrDefault()!.GetUserGeometryInfosAsync(names);
        }
    }
}
