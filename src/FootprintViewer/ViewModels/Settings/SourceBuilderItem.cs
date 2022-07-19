using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public enum SourceType
    {
        File,
        Folder,
        Database,
        Random,
    };

    public interface ISourceBuilderItem
    {
        public string Name { get; }

        ReactiveCommand<Unit, ISourceViewModel?> Build { get; }

        Interaction<ISourceViewModel, ISourceViewModel?> BuildDialog { get; }
    }

    public class SourceBuilderItem : ISourceBuilderItem
    {
        private readonly SourceType _type;
        private readonly Func<ISourceViewModel> _builder;

        public SourceBuilderItem(SourceType type, Func<ISourceViewModel> builder)
        {
            _type = type;

            _builder = builder;

            BuildDialog = new Interaction<ISourceViewModel, ISourceViewModel?>();

            Build = ReactiveCommand.CreateFromTask(BuildAsync);
        }

        private async Task<ISourceViewModel?> BuildAsync() => await BuildDialog.Handle(_builder.Invoke());

        public ReactiveCommand<Unit, ISourceViewModel?> Build { get; }

        public string Name => _type.ToString();

        public Interaction<ISourceViewModel, ISourceViewModel?> BuildDialog { get; }
    }
}
