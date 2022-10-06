using DataSettingsSample.ViewModels.Interfaces;
using ReactiveUI;
using System.Reactive;

namespace DataSettingsSample.ViewModels
{
    public abstract class BaseSourceBuilderViewModel : ReactiveObject, ISourceBuilderViewModel
    {
        public BaseSourceBuilderViewModel()
        {
            Cancel = ReactiveCommand.Create(CancelImpl, outputScheduler: RxApp.MainThreadScheduler);
        }

        protected ISourceViewModel AddImpl()
        {
            return new SourceViewModel();
        }

        protected void CancelImpl()
        {

        }

        public abstract ReactiveCommand<Unit, ISourceViewModel> Add { get; }

        public ReactiveCommand<Unit, Unit> Cancel { get; }
    }
}
