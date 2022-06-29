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

        ReactiveCommand<Unit, ISourceInfo?> Build { get; }

        Interaction<ISourceInfo, ISourceInfo?> BuildDialog { get; }
    }

    public class SourceBuilderItem : ISourceBuilderItem
    {
        private readonly SourceType _type;
        private readonly Func<ISourceInfo> _builder;

        public SourceBuilderItem(SourceType type, Func<ISourceInfo> builder)
        {
            _type = type;

            _builder = builder;

            BuildDialog = new Interaction<ISourceInfo, ISourceInfo?>();

            Build = ReactiveCommand.CreateFromTask(BuildAsync);
        }

        private async Task<ISourceInfo?> BuildAsync() => await BuildDialog.Handle(_builder.Invoke());

        public ReactiveCommand<Unit, ISourceInfo?> Build { get; }

        public string Name => _type.ToString();

        public Interaction<ISourceInfo, ISourceInfo?> BuildDialog { get; }
    }
}
