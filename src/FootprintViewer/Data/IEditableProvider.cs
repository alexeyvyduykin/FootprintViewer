using ReactiveUI;
using System.Reactive;
using System.Threading.Tasks;

namespace FootprintViewer.Data
{
    public interface IEditableProvider<TNative> : IProvider<TNative>
    {
        ReactiveCommand<Unit, Unit> Update { get; }

        Task AddAsync(TNative value);

        Task RemoveAsync(TNative value);

        Task EditAsync(string key, TNative value);
    }
}
