using ReactiveUI;
using System.Reactive;

namespace DataSettingsSample.ViewModels
{
    public class SourceBuilderViewModel : ReactiveObject
    {
        public SourceBuilderViewModel()
        {
            Add = ReactiveCommand.Create(AddImpl, outputScheduler: RxApp.MainThreadScheduler);
            Cancel = ReactiveCommand.Create(CancelImpl, outputScheduler: RxApp.MainThreadScheduler);
        }

        protected SourceViewModel AddImpl()
        {
            return new SourceViewModel();
        }

        protected void CancelImpl()
        {

        }

        public ReactiveCommand<Unit, SourceViewModel> Add { get; set; }

        public ReactiveCommand<Unit, Unit> Cancel { get; set; }
    }
}
