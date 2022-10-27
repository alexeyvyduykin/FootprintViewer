using DataSettingsSample.Data;

namespace DataSettingsSample.ViewModels.Interfaces
{
    public interface ISourceViewModel
    {
        ISource Source { get; }

        string Name { get; set; }
    }
}
