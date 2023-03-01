using FootprintViewer.Data;

namespace FootprintViewer.ViewModels;

public interface ISourceViewModel
{
    string? Name { get; set; }

    ISource Source { get; }
}
