using FootprintViewer.Data.Sources;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<GroundTarget> GetGroundTargets()
        {
            var list = new List<GroundTarget>();

            foreach (var source in Sources)
            {
                list.AddRange(source.GetGroundTargetsAsync().Result);
            }

            return list;
        }

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
    }
}
