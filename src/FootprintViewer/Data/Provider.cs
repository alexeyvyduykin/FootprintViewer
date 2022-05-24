﻿using FootprintViewer.ViewModels;
using ReactiveUI;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.Data
{
    public interface IProvider<T>
    {
        ReactiveCommand<IFilter<T>?, List<T>> Loading { get; }

        Task<List<T>> GetValuesAsync(IFilter<T>? filter = null);
    }

    public abstract class BaseProvider<T>
    {
        private readonly IList<T> _sources = new List<T>();

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
            Loading = ReactiveCommand.CreateFromTask<IFilter<T>?, List<T>>(filter => GetValuesAsync(filter));
        }

        public Provider(IDataSource<T>[] sources) : this()
        {
            foreach (var item in sources)
            {
                AddSource(item);
            }
        }

        public ReactiveCommand<IFilter<T>?, List<T>> Loading { get; }

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