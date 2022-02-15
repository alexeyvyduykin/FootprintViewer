using FootprintViewer.Data.Sources;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Threading.Tasks;

namespace FootprintViewer.Data
{
    public class GroundTargetProvider : BaseProvider<IGroundTargetDataSource>
    {
        public GroundTargetProvider()
        {
            Loading = ReactiveCommand.CreateFromTask(GetGroundTargetsAsync);
        }

        public ReactiveCommand<Unit, List<GroundTarget>> Loading { get; }

        public async Task<List<GroundTarget>> GetGroundTargetsAsync()
        {
            return await Sources.First().GetGroundTargetsAsync();

            //var list = new List<GroundTarget>();

            //foreach (var source in Sources)
            //{
            //    list.AddRange(await source.GetGroundTargetsAsync());
            //}

            //return list;
        }

        public async Task<List<GroundTargetInfo>> GetGroundTargetInfosAsync(string[] names)
        {
            return await Sources.First().GetGroundTargetInfosAsync(names);
        }

        public async Task<List<GroundTargetInfo>> GetGroundTargetInfosExAsync(Func<GroundTarget, bool> func)
        {
            return await Sources.First().GetGroundTargetInfosExAsync(func);
        }
    }
}
