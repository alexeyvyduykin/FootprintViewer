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
    public class UserGeometryProvider : BaseProvider<IUserGeometryDataSource>, IEditableProvider<UserGeometryInfo>
    {
        public UserGeometryProvider()
        {
            Update = ReactiveCommand.Create(() => { });

            Loading = ReactiveCommand.CreateFromTask<IFilter<UserGeometryInfo>?, List<UserGeometryInfo>>(GetValuesAsync);
        }

        public ReactiveCommand<IFilter<UserGeometryInfo>?, List<UserGeometryInfo>> Loading { get; }

        public ReactiveCommand<Unit, Unit> Update { get; }

        public async Task UpdateGeometry(string key, NetTopologySuite.Geometries.Geometry geometry)
        {
            await Sources.FirstOrDefault()!.UpdateGeometry(key, geometry);
        }

        public async Task AddAsync(UserGeometryInfo value)
        {
            foreach (var source in Sources)
            {
                await source.AddAsync(value.Geometry);
            }

            Update.Execute().Subscribe();
        }

        public async Task RemoveAsync(UserGeometryInfo value)
        {
            foreach (var source in Sources)
            {
                await source.RemoveAsync(value.Geometry);
            }

            Update.Execute().Subscribe();
        }

        public async Task<List<UserGeometryInfo>> GetValuesAsync(IFilter<UserGeometryInfo>? filter = null)
        {
            return await Sources.FirstOrDefault()!.GetUserGeometryInfosAsync(filter);

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
    }
}
