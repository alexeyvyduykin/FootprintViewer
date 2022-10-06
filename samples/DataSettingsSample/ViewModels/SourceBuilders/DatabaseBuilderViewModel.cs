using DataSettingsSample.ViewModels.Interfaces;
using ReactiveUI;
using System.Reactive;

namespace DataSettingsSample.ViewModels
{
    public class DatabaseBuilderViewModel : BaseSourceBuilderViewModel, IDatabaseBuilderViewModel
    {
        public override ReactiveCommand<Unit, ISourceViewModel> Add => throw new System.NotImplementedException();
    }
}
