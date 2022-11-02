using ReactiveUI;
using System;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace FootprintViewer.Data
{
    //public class EditableProvider<TNative> : Provider<TNative>, IEditableProvider<TNative>
    //{
    //    public EditableProvider() : base()
    //    {
    //        Update = ReactiveCommand.Create(() => { });
    //    }

    //    public EditableProvider(IDataSource[] sources) : base(sources)
    //    {
    //        Update = ReactiveCommand.Create(() => { });
    //    }

    //    public ReactiveCommand<Unit, Unit> Update { get; }

    //    public async Task EditAsync(string key, TNative value)
    //    {
    //        foreach (var source in GetSources())
    //        {
    //            var sourceType = source.GetType().GetInterfaces().First();

    //            var manager = _managers.Where(s =>
    //            {
    //                var res = s.GetType().BaseType?.GetGenericArguments()[1];
    //                return Equals(res, sourceType);
    //            }).FirstOrDefault();

    //            if (manager != null && manager is IEditableDataManager<TNative> editable)
    //            {
    //                await editable.EditAsync(source, key, value);
    //            }
    //        }

    //        Update.Execute().Subscribe();
    //    }

    //    public async Task AddAsync(TNative value)
    //    {
    //        foreach (var source in GetSources())
    //        {
    //            var sourceType = source.GetType().GetInterfaces().First();

    //            var manager = _managers.Where(s =>
    //            {
    //                var res = s.GetType().BaseType?.GetGenericArguments()[1];
    //                return Equals(res, sourceType);
    //            }).FirstOrDefault();

    //            if (manager != null && manager is IEditableDataManager<TNative> editable)
    //            {
    //                await editable.AddAsync(source, value);
    //            }
    //        }

    //        Update.Execute().Subscribe();
    //    }

    //    public async Task RemoveAsync(TNative value)
    //    {
    //        foreach (var source in GetSources())
    //        {
    //            var sourceType = source.GetType().GetInterfaces().First();

    //            var manager = _managers.Where(s =>
    //            {
    //                var res = s.GetType().BaseType?.GetGenericArguments()[1];
    //                return Equals(res, sourceType);
    //            }).FirstOrDefault();

    //            if (manager != null && manager is IEditableDataManager<TNative> editable)
    //            {
    //                await editable.RemoveAsync(source, value);
    //            }
    //        }

    //        Update.Execute().Subscribe();
    //    }
    //}
}
