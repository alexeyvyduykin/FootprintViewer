using ReactiveUI;
using System;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace FootprintViewer.Data
{
    public interface IEditableProvider<T> : IProvider<T>
    {
        ReactiveCommand<Unit, Unit> Update { get; }

        Task AddAsync(T value);

        Task RemoveAsync(T value);

        Task EditAsync(string key, T value);
    }

    public class EditableProvider<T> : Provider<T>, IEditableProvider<T>
    {
        public EditableProvider() : base()
        {
            Update = ReactiveCommand.Create(() => { });
        }

        public EditableProvider(IDataSource<T>[] sources) : base(sources)
        {
            Update = ReactiveCommand.Create(() => { });
        }

        public ReactiveCommand<Unit, Unit> Update { get; }

        public async Task EditAsync(string key, T value)
        {
            foreach (var source in Sources.Where(s => s is IEditableDataSource<T>).Cast<IEditableDataSource<T>>())
            {
                await source.EditAsync(key, value);
            }

            Update.Execute().Subscribe();
        }

        public async Task AddAsync(T value)
        {
            foreach (var source in Sources.Where(s => s is IEditableDataSource<T>).Cast<IEditableDataSource<T>>())
            {
                await source.AddAsync(value);
            }

            Update.Execute().Subscribe();
        }

        public async Task RemoveAsync(T value)
        {
            foreach (var source in Sources.Where(s => s is IEditableDataSource<T>).Cast<IEditableDataSource<T>>())
            {
                await source.RemoveAsync(value);
            }

            Update.Execute().Subscribe();
        }
    }
}
