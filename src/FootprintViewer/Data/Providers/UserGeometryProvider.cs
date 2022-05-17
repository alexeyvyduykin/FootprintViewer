using FootprintViewer.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace FootprintViewer.Data
{
    public class UserGeometryProvider : BaseProvider<IEditableDataSource<UserGeometryInfo>>, IEditableProvider<UserGeometryInfo>
    {
        public UserGeometryProvider()
        {
            Update = ReactiveCommand.Create(() => { });

            Loading = ReactiveCommand.CreateFromTask<IFilter<UserGeometryInfo>?, List<UserGeometryInfo>>(GetValuesAsync);
        }

        public ReactiveCommand<IFilter<UserGeometryInfo>?, List<UserGeometryInfo>> Loading { get; }

        public ReactiveCommand<Unit, Unit> Update { get; }

        public async Task EditAsync(string key, UserGeometryInfo value)
        {
            await Sources.FirstOrDefault()!.EditAsync(key, value);
        }

        public async Task AddAsync(UserGeometryInfo value)
        {
            foreach (var source in Sources)
            {
                await source.AddAsync(value);
            }

            Update.Execute().Subscribe();
        }

        public async Task RemoveAsync(UserGeometryInfo value)
        {
            foreach (var source in Sources)
            {
                await source.RemoveAsync(value);
            }

            Update.Execute().Subscribe();
        }

        public async Task<List<UserGeometryInfo>> GetValuesAsync(IFilter<UserGeometryInfo>? filter = null)
        {
            return await Sources.FirstOrDefault()!.GetValuesAsync(filter);
        }
    }
}
