using FootprintViewer.Data.DataManager;

namespace FootprintViewer.ViewModels;

public interface ISourceViewModel
{
    string? Name { get; set; }

    ISource Source { get; }
}
