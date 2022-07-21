using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data
{
    public class Provider<TNative> : IProvider<TNative>
    {
        protected List<IDataSource> _sources = new();
        protected readonly List<IDataManager<TNative>> _managers = new();

        public Provider()
        {
            _sources = new List<IDataSource>();
            _managers = new List<IDataManager<TNative>>();

            UpdateSources = ReactiveCommand.CreateFromObservable(() => Observable.Start(() => { }));
        }

        public Provider(IDataSource[] sources) : this()
        {
            foreach (var item in sources)
            {
                AddSource(item);
            }
        }

        public ReactiveCommand<Unit, Unit> UpdateSources { get; }

        public IEnumerable<IDataSource> GetSources() => _sources;

        public async Task<List<TNative>> GetNativeValuesAsync(IFilter<TNative>? filter)
        {
            return await Task.Run(async () =>
            {
                var list = new List<TNative>();

                foreach (var source in Sources)
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

                foreach (var source in Sources)
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

        public void AddSource(IDataSource source)
        {
            _sources.Add(source);
        }

        public void AddSources(IEnumerable<IDataSource> sources)
        {
            _sources.AddRange(sources);
        }

        public void AddManagers(IEnumerable<IDataManager<TNative>> managers)
        {
            _managers.AddRange(managers);
        }

        protected IEnumerable<IDataSource> Sources => _sources;

        public void ChangeSources(IDataSource[] sources)
        {
            _sources = new List<IDataSource>(sources);

            UpdateSources.Execute().Subscribe();
        }
    }
}
