using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data
{
    public class Provider<TNative> : IProvider<TNative>
    {
        protected readonly List<IDataManager<TNative>> _managers = new();

        public Provider()
        {
            _managers = new List<IDataManager<TNative>>();
        }

        public Provider(IDataSource[] sources) : this()
        {
            Sources.AddRange(sources);
        }

        public IObservable<IChangeSet<IDataSource>> Observable => Sources.Connect();

        public SourceList<IDataSource> Sources { get; } = new();

        protected IEnumerable<IDataSource> GetSources() => Sources.Items;

        public async Task<List<TNative>> GetNativeValuesAsync(IFilter<TNative>? filter)
        {
            return await Task.Run(async () =>
            {
                var list = new List<TNative>();

                foreach (var source in GetSources())
                {
                    var sourceType = source.GetType().GetInterfaces().First();

                    var manager = _managers.Where(s =>
                    {
                        var res = s.GetType().BaseType?.GetGenericArguments()[1];
                        return Equals(res, sourceType);
                    }).FirstOrDefault();

                    if (manager != null)
                    {
                        var values = await manager.GetNativeValuesAsync(source, filter);

                        list.AddRange(values);
                    }
                }

                return list;
            });
        }

        public async Task<List<T>> GetValuesAsync<T>(IFilter<T>? filter, Func<TNative, T> converter)
        {
            return await Task.Run(async () =>
            {
                var list = new List<T>();

                foreach (var source in GetSources())
                {
                    var sourceType = source.GetType().GetInterfaces().First();

                    var manager = _managers.Where(s =>
                    {
                        var res = s.GetType().BaseType?.GetGenericArguments()[1];
                        return Equals(res, sourceType);
                    }).FirstOrDefault();

                    if (manager != null)
                    {
                        var values = await manager.GetValuesAsync(source, filter, converter);

                        list.AddRange(values);
                    }
                }

                return list;
            });
        }

        public void AddSources(IEnumerable<IDataSource> sources)
        {
            foreach (var item in sources)
            {
                var count = Sources.Items.Where(s => Equals(s, item)).Count();

                if (count == 0)
                {
                    Sources.Add(item);
                }
            }
        }

        public void RemoveSources(IEnumerable<IDataSource> sources)
        {
            foreach (var item in sources)
            {
                Sources.Remove(item);
            }
        }

        public void AddManagers(IEnumerable<IDataManager<TNative>> managers)
        {
            _managers.AddRange(managers);
        }
    }
}
