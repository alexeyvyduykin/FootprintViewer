using ReactiveUI;
using System;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace FootprintViewer.Data
{
    public class EditableProvider<TNative> : Provider<TNative>, IEditableProvider<TNative>
    {
        public EditableProvider() : base()
        {
            Update = ReactiveCommand.Create(() => { });
        }

        public EditableProvider(IDataSource<TNative>[] sources) : base(sources)
        {
            Update = ReactiveCommand.Create(() => { });
        }

        public ReactiveCommand<Unit, Unit> Update { get; }

        public async Task EditAsync(string key, TNative value)
        {
            foreach (var source in Sources.Where(s => s is IEditableDataSource<TNative>).Cast<IEditableDataSource<TNative>>())
            {
                await source.EditAsync(key, value);
            }

            Update.Execute().Subscribe();
        }

        public async Task AddAsync(TNative value)
        {
            foreach (var source in Sources.Where(s => s is IEditableDataSource<TNative>).Cast<IEditableDataSource<TNative>>())
            {
                await source.AddAsync(value);
            }

            Update.Execute().Subscribe();
        }

        public async Task RemoveAsync(TNative value)
        {
            foreach (var source in Sources.Where(s => s is IEditableDataSource<TNative>).Cast<IEditableDataSource<TNative>>())
            {
                await source.RemoveAsync(value);
            }

            Update.Execute().Subscribe();
        }
    }
}
