using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive;

namespace DataSettingsSample.ViewModels
{
    public class SourceBuilderItemViewModel : ReactiveObject
    {
        public SourceBuilderItemViewModel()
        {
            Command = ReactiveCommand.Create(CommandImpl, outputScheduler: RxApp.MainThreadScheduler);
        }

        protected void CommandImpl()
        {

        }

        [Reactive]
        public string? Name { get; set; }

        public ReactiveCommand<Unit, Unit> Command { get; set; }
    }
}
