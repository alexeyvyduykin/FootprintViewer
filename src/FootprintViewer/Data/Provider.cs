using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data
{
    public abstract class BaseProvider<TSource>
    {
        protected List<TSource> _sources = new();

        public void AddSource(TSource source)
        {
            _sources.Add(source);
        }

        protected IEnumerable<TSource> Sources => _sources;
    }

    public class Provider<TNative> : BaseProvider<IDataSource<TNative>>, IProvider<TNative>
    {
        public Provider()
        {
            _sources = new List<IDataSource<TNative>>();

            UpdateSources = ReactiveCommand.CreateFromObservable(() => Observable.Start(() => { }));
        }

        public Provider(IDataSource<TNative>[] sources) : this()
        {
            foreach (var item in sources)
            {
                AddSource(item);
            }
        }

        public ReactiveCommand<Unit, Unit> UpdateSources { get; }

        public void ChangeSources(IDataSource<TNative>[] sources)
        {
            _sources = new List<IDataSource<TNative>>(sources);

            UpdateSources.Execute().Subscribe();
        }

        public async Task<List<TNative>> GetNativeValuesAsync(IFilter<TNative>? filter)
        {
            return await Task.Run(async () =>
            {
                var list = new List<TNative>();

                foreach (var source in Sources)
                {
                    var values = await source.GetNativeValuesAsync(filter);

                    list.AddRange(values);
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
                    var values = await source.GetValuesAsync<T>(filter, converter);

                    list.AddRange(values);
                }

                return list;
            });
        }
    }
}
