using FootprintViewer.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data
{
    public interface IProvider<T>
    {
        Task<List<T>> GetValuesAsync(IFilter<T>? filter = null);
    }

    public abstract class BaseProvider<T>
    {
        protected List<T> _sources = new();

        public void AddSource(T source)
        {
            _sources.Add(source);
        }

        protected IEnumerable<T> Sources => _sources;
    }

    public class Provider<T> : BaseProvider<IDataSource<T>>, IProvider<T>
    {
        public Provider()
        {
            _sources = new List<IDataSource<T>>();

            UpdateSources = ReactiveCommand.CreateFromObservable(() => Observable.Start(() => { }));
        }

        public Provider(IDataSource<T>[] sources) : this()
        {
            foreach (var item in sources)
            {
                AddSource(item);
            }
        }

        public ReactiveCommand<Unit, Unit> UpdateSources { get; }

        public void ChangeSources(IDataSource<T>[] sources)
        {
            _sources = new List<IDataSource<T>>(sources);

            UpdateSources.Execute().Subscribe();
        }

        public async Task<List<T>> GetValuesAsync(IFilter<T>? filter)
        {
            return await Task.Run(async () =>
            {
                var list = new List<T>();

                foreach (var source in Sources)
                {
                    var values = await source.GetValuesAsync(filter);

                    list.AddRange(values);
                }

                return list;
            });
        }
    }
}
