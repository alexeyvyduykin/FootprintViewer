using ReactiveUI;
using System.Reactive;

namespace DataSettingsSample.ViewModels.Interfaces
{
    public interface ISourceBuilderViewModel
    {
        ReactiveCommand<Unit, ISourceViewModel> Add { get; }

        ReactiveCommand<Unit, Unit> Cancel { get; }
    }
}
