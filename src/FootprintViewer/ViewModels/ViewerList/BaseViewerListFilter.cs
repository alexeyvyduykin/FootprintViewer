using ReactiveUI;
using System.Reactive;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public interface IFilter<T>
    {
        bool Filtering(T value);

        string[]? Names { get; }

        ReactiveCommand<Unit, IFilter<T>> Update { get; }

        ReactiveCommand<Unit, Unit> Init { get; }
    }

    public abstract class ViewerListFilter<T> : ReactiveObject, IFilter<T> where T : IViewerItem
    {
        public ViewerListFilter()
        {
            Init = ReactiveCommand.CreateFromTask(InitImpl);

            Update = ReactiveCommand.Create<IFilter<T>>(() => this);
        }

        protected virtual async Task InitImpl()
        {
            await Task.Run(() => { });
        }

        public abstract string[]? Names { get; }

        public abstract bool Filtering(T value);

        public ReactiveCommand<Unit, IFilter<T>> Update { get; }

        public ReactiveCommand<Unit, Unit> Init { get; }
    }
}
